<template>
  <div id="reportFilters">
    <div>
      <label for="selectCompany" class="form-label fw-bold">Empresa</label>
      <select id="selectCompany" class="form-select" v-model.number="selectedCompanyId">
        <option :value="0">Todas</option>
        <option 
        v-for="company in companies" 
        :key="company.companyUniqueId" 
        :value="company.companyUniqueId"
        >
        {{ company.companyName }}
      </option>
    </select>
  </div>
  
  <div>
    <label for="dateFrom" class="form-label fw-bold">Fecha inicial</label>
    <input id="dateFrom" type="date" class="form-control" v-model="dateFrom"/>
  </div>
  
  <div>
    <label for="dateTo" class="form-label fw-bold">Fecha final</label>
    <input id="dateTo" type="date" class="form-control" v-model="dateTo"/>
  </div>
</div>

<div>
  <div id="buttons">
    <LinkButton text="Descargar Excel" @click="downloadExcel()" />
  </div>
</div>

<div id="reportContent">
  <h4>Reporte histórico pago de planilla</h4>
  
  <p>Empresa: {{ companyFilterLabel }}</p>
  <p>Fecha inicial: {{ dateFrom }}</p>
  <p>Fecha final: {{ dateTo }}</p>

  <div v-if="isLoading" class="text-muted">Cargando reporte…</div>
  
  <div v-else id="reportTable" class="table-responsive">
    <table class="table">
      <thead>
        <tr>
          <th>Nombre de empresa</th>
          <th>Frecuencia de pago</th>
          <th>Periodo de pago</th>
          <th>Fecha de pago</th>
          <th>Salario bruto</th>
          <th>Cargas sociales empleador</th>
          <th>Beneficios a empleados</th>
          <th>Costo empleador</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(row, index) in payrollData" :key="index">
          <td>{{ row.CompanyName }}</td>
          <td>{{ row.PaymentFrequency }}</td>
          <td>{{ row.Period }}</td>
          <td>{{ formatDate(row.PaymentDate) }}</td>
          <td>{{ fmtCRC(row.GrossSalary) }}</td>
          <td>{{ fmtCRC(row.EmployerContributions) }}</td>
          <td>{{ fmtCRC(row.EmployeeBenefits) }}</td>
          <td>{{ fmtCRC(row.EmployerCost) }}</td>
        </tr>
        <tr v-if="!payrollData.length">
            <td colspan="8" class="text-center text-muted">
              No hay datos para mostrar.
            </td>
          </tr>
      </tbody>
    </table>
  </div>
</div>
</template>

<script>
import URLBaseAPI from '../../axiosAPIInstances.js';
import LinkButton from '../LinkButton.vue';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

export default {
  components: {
    LinkButton,
  },
  data() {
    return {
      companies: [],
      selectedCompanyId: 0,
      dateFrom: '',
      dateTo: '',
      loadingCompanies: false,
      isLoading: false,
      showToast: false,
      toastMessage: '',
      toastType: '',
      payrollData: [],
    };
  },
  computed: {
    companyFilterLabel() {
      if (!this.selectedCompanyId || this.selectedCompanyId === 0) return 'Todas';
      const match = this.companies.find(c => c.companyUniqueId === this.selectedCompanyId);
      return match?.companyName || '';
    },
  },
  methods: {
    async fetchCompanies() {
      this.loadingCompanies = true;
      this.toastMessage = '';
      try {
        const userId = this.$session.user?.userId;
        
        if (!userId) {
          this.companies = [];
          return;
        }
        
        const { data } = await URLBaseAPI.get(
        `/api/Company/by-user/${userId}?onlyActive=false`
        );
        
        const rows = Array.isArray(data) ? data.slice() : [];
        this.companies = rows;
      } catch (err) {
        this.toastType = 'bg-danger';
        this.toastMessage = 'Error al cargar empresas para el filtro.';
        this.showToast = true;
      } finally {
        this.loadingCompanies = false;
      }
    },
    async loadReport() {
      const employeeId = Number(this.$session.user?.personId);
      const params = {
        reportCode: 'EmployerHistoryPayroll',
        companyId: this.selectedCompanyId || 0,
        employeeId,
        dateFrom: this.dateFrom,
        dateTo: this.dateTo,
      };

      this.isLoading = true;
      this.payrollData = [];
      try {
        const { data } = await URLBaseAPI.post('/api/Reports/data', params);
        const rows = Array.isArray(data.rows) ? data.rows : [];
        this.payrollData = rows;
      } catch (error) {
        const data = error && error.response && error.response.data ? error.response.data : null;
        const msg =
          typeof data === 'string'
            ? data
            : (data && (data.message || data.detail)) || 'Error cargando el reporte histórico de empresas';
        this.toastMessage = msg;
        this.toastType = 'bg-danger';
        this.showToast = true;

        setTimeout(function () {
          this.showToast = false;
        }, this.toastTimeout);
        this.reportResult = null;
      } finally {
        this.isLoading = false;
      }
    },
    downloadExcel() {
      const tableWrapper = document.getElementById('reportTable');
      if (!tableWrapper) return;
      
      const table = tableWrapper.querySelector('table');
      if (!table) return;
      
      const exportTable = table.cloneNode(true);
      const numericCols = [4, 5, 6, 7];
      
      const rows = exportTable.querySelectorAll('tbody tr');
      rows.forEach(tr => {
        const cells = tr.querySelectorAll('td');
        
        numericCols.forEach(idx => {
          const cell = cells[idx];
          if (!cell) return;
          const raw = cell.textContent || '';
          let txt = raw.replace(/[^\d.,-]/g, '');
          const seps = txt.match(/[.,]/g);
          if (seps && seps.length > 1) {
            const lastSep = Math.max(txt.lastIndexOf(','), txt.lastIndexOf('.'));
            const intPart = txt.slice(0, lastSep).replace(/[.,]/g, '');
            const decPart = txt.slice(lastSep + 1).replace(/[.,]/g, '');
            txt = intPart + '.' + decPart;
          } else {
            txt = txt.replace(/\./g, '').replace(',', '.');
          }
          
          cell.textContent = txt;
        });
      });
      
      const wb = XLSX.utils.table_to_book(exportTable, { sheet: 'Planilla' });
      const wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
      const blob = new Blob([wbout], { type: 'application/octet-stream' });
      saveAs(blob, 'Reporte_Planilla.xlsx');
    },
    fmtCRC(v) {
      return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: 'CRC',
        currencyDisplay: 'symbol',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }).format(Number(v ?? 0));
    },
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return isNaN(date)
        ? ''
        : new Intl.DateTimeFormat('es-CR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
          }).format(date);
    },
  },
  watch: {
    selectedCompanyId() {
      this.loadReport();
    },
    dateFrom() {
      this.loadReport();
    },
    dateTo() {
      this.loadReport();
    },
  },
  mounted() {
    const today = new Date();
    const first = new Date(today.getFullYear(), today.getMonth(), 1);
    const last = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    
    this.dateFrom = first.toISOString().slice(0, 10);
    this.dateTo = last.toISOString().slice(0, 10);
    
    this.fetchCompanies().then(() => {
      this.loadReport();
    });
  },
};
</script>

<style lang="scss" scoped>
#reportFilters {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
}

#reportFilters > div {
  width: 100%;
  text-align: left;
}

#webpageLayout > main > div > div.text-center > div:nth-child(2) {
  text-align: left;
}

#buttons {
  display: inline-block;
  margin-bottom: 20px;
}

#reportContent {
  background: rgba(255, 255, 255, 0.9);
  border-radius: 10px;
  
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  
  padding: 20px;
}

h4, p {
  color: #000;
}

#reportTable {
  width: 100%;
  border-radius: 10px;
}

#reportTable th, #reportTable td {
  text-align: left;
  padding-left: 10px;
}

#reportTable th {
  background-color: #1C4532;
  color: white;
  font-weight: normal;
}
</style>