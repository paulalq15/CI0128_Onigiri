<template>
  <div id="reportFilters">
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
    
    <p><strong>Empresa:</strong> {{ companyName }}</p>
    <p><strong>Nombre del empleado:</strong> {{ employeeName }}</p>

    <p><strong>Fecha inicial:</strong> {{ formatFilterDate(dateFrom) }}</p>
    <p><strong>Fecha final:</strong> {{ formatFilterDate(dateTo) }}</p>

    <div id="reportTable" class="table-responsive">
      <table class="table">
        <thead>
          <tr>
            <th>Tipo de contrato</th>
            <th>Posición</th>
            <th>Fecha de pago</th>
            <th>Salario bruto</th>
            <th>Deducciones obligatorias</th>
            <th>Deducciones voluntarias</th>
            <th>Salario neto</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(row, index) in payrollData.slice(0, -1)" :key="index">
            <td>{{ row.contractType }}</td>
            <td>{{ row.position }}</td>
            <td>{{ formatDate(row.paymentDate) }}</td>
            <td>{{ fmtCRC(row.grossSalary) }}</td>
            <td>{{ fmtCRC(row.mandatoryDeductions) }}</td>
            <td>{{ fmtCRC(row.voluntaryDeductions) }}</td>
            <td>{{ fmtCRC(row.netSalary) }}</td>
          </tr>

          <tr v-if="!payrollData.length">
            <td colspan="7" class="text-center text-muted">
              No hay datos para mostrar.
            </td>
          </tr>

          <!-- Fila de totales -->
          <tr v-if="payrollData.length">
            <td colspan="3"><strong>Total</strong></td>
            <td>{{ fmtCRC(payrollData.at(-1).totalGrossSalary) }}</td>
            <td>{{ fmtCRC(payrollData.at(-1).totalLegalDeductions) }}</td>
            <td>{{ fmtCRC(payrollData.at(-1).totalVoluntaryDeductions) }}</td>
            <td>{{ fmtCRC(payrollData.at(-1).totalNetSalary) }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
  import * as XLSX from "xlsx";
  import { saveAs } from "file-saver";
  import { useSession } from '@/utils/useSession';
  import { useGlobalAlert } from '@/utils/alerts.js';

  import URLBaseAPI from '../../axiosAPIInstances.js';
  import LinkButton from '../LinkButton.vue';

  export default {
    components: {
      LinkButton,
    },
    data() {
      const { user } = useSession();

      return {
        user,
        companyName: "",
        employeeName: "",
        dateFrom: '',
        dateTo: '',
        payrollData: [],
        isLoading: false,
      };
    },
    methods: {
      async loadReport() {
        if (!this.dateFrom || !this.dateTo) return;

        this.companyName = this.user?.companyName || '';
        this.employeeName = this.user?.fullName || '';
        const companyId = this.user?.companyUniqueId;
        const employeeId = Number(this.user?.personId);

        const params = {
          reportCode: 'EmployeeHistoryPayroll',
          companyId,
          employeeId,
          dateFrom: this.dateFrom,
          dateTo: this.dateTo,
        };

        this.isLoading = true;
        this.payrollData = [];

        try {
          const response = await URLBaseAPI.post('/api/Reports/data', params);

          this.companyName = response.data.reportInfo.CompanyName;
          this.employeeName = response.data.reportInfo.EmployeeName;

          this.payrollData = response.data.rows || [];

        } catch (error) {
          const data = error?.response?.data;
          const msg =
            typeof data === 'string' ? data
            : (data && (data.message || data.detail)) || 'Error cargando el detalle de planilla';
          
          const noDataMsg = 'No se encontró información de pago para el empleado en el rango seleccionado';
          if (msg !== noDataMsg) {
            const alert = useGlobalAlert();
            alert.show(msg, "warning");
          }

          this.payrollData = [];
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
      formatFilterDate(value) {
        if (!value) return '';
        const [yyyy, mm, dd] = value.split('-');
        return `${dd}/${mm}/${yyyy}`;
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
      dateFrom() {
        this.loadReport();
      },
      dateTo() {
        this.loadReport();
      },
    },
    async mounted() {
      const today = new Date();
      const first = new Date(today.getFullYear(), today.getMonth(), 1);
      const last = new Date(today.getFullYear(), today.getMonth() + 1, 0);
      this.dateFrom = first.toISOString().slice(0, 10);
      this.dateTo = last.toISOString().slice(0, 10);
      this.loadReport();
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
    background: white;
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

  .totals-row td {
    font-weight: 600;
    border-top: 2px solid #1C4532;
  }
</style>