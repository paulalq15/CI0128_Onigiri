<template>
  <div id="reportFilters" v-if="isReportLoaded">
    <!-- Fecha inicial -->
    <div>
      <label for="selectFechaInicial" class="form-label fw-bold">Fecha inicial</label>
      <select
        id="selectFechaInicial"
        class="form-select"
        v-model="selectedInitialDate"
        @change="loadReport()"
      >
        <option
          v-for="date in initialDatesList"
          :key="date.payrollId"
          :value="date"
        >
          {{ date.periodLabel }}
        </option>
      </select>
    </div>

    <!-- Fecha final -->
    <div>
      <label for="selectFechaFinal" class="form-label fw-bold">Fecha final</label>
      <select
        id="selectFechaFinal"
        class="form-select"
        v-model="selectedFinalDate"
        @change="loadReport()"
      >
        <option
          v-for="date in finalDatesList"
          :key="date.payrollId"
          :value="date"
        >
          {{ date.periodLabel }}
        </option>
      </select>
    </div>
  </div>

  <div>
    <div id="buttons" v-if="isReportLoaded">
      <LinkButton text="Descargar Excel" @click="downloadExcel()" />
    </div>
  </div>

  <div v-if="!isReportLoaded">
    {{ waitingMessage }}
  </div>
  <div id="reportContent" v-else>
    <h4>Reporte histórico pago de planilla</h4>
    
    <p><strong>Empresa:</strong> {{ companyName }}</p>
    <p><strong>Nombre del empleado:</strong> {{ employeeName }}</p>

    <p><strong>Fecha inicial:</strong> {{ selectedInitialDate.periodLabel }}</p>
    <p><strong>Fecha final:</strong> {{ selectedFinalDate.periodLabel }}</p>

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
            <td>{{ row.paymentDate }}</td>
            <td>{{ row.grossSalary }}</td>
            <td>{{ row.mandatoryDeductions }}</td>
            <td>{{ row.voluntaryDeductions }}</td>
            <td>{{ row.netSalary }}</td>
          </tr>

          <!-- Fila de totales -->
          <tr>
            <td colspan="3"><strong>Total</strong></td>
            <td>{{ payrollData.at(-1).totalGrossSalary }}</td>
            <td>{{ payrollData.at(-1).totalLegalDeductions }}</td>
            <td>{{ payrollData.at(-1).totalVoluntaryDeductions }}</td>
            <td>{{ payrollData.at(-1).totalNetSalary }}</td>
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

        initialDatesList: [],
        selectedInitialDate: null,

        waitingMessage: "Cargando datos...",

        selectedFinalDate: null,

        payrollData: [],

        isReportLoaded: false,
      };
    },
    computed: {
      finalDatesList() {
        if (!this.selectedInitialDate) return [];

        const index = this.initialDatesList.findIndex(
          item => item.payrollId === this.selectedInitialDate.payrollId
        );

        if (index === -1) return [];

        return this.initialDatesList.slice(index);
      },
    },
    methods: {
      async getPayrollPeriodsList() {
        const companyId = this.user?.companyUniqueId;
        const employeeId = Number(this.user?.personId);

        try {
          const response = await URLBaseAPI.get('/api/Reports/employee/payroll-periods', {
            params: { companyId, employeeId }
          });

          this.initialDatesList = response.data || [];

          if (this.initialDatesList.length > 0) {
            this.selectedInitialDate = this.initialDatesList[0];
            this.selectedFinalDate = this.initialDatesList[this.initialDatesList.length - 1];
          }
        } catch (error) {
          const data = error?.response?.data;
          const msg =
            typeof data === "string" ? data
            : data?.message || data?.detail || "Error cargando las últimas planillas";

          const alert = useGlobalAlert();
          alert.show(msg, "warning");
        }
      },

      async loadReport() {
        if (!this.selectedInitialDate || !this.selectedFinalDate) {
          this.waitingMessage = "No hay datos que mostrar";
          return;
        }

        const companyId = this.user?.companyUniqueId;
        const employeeId = Number(this.user?.personId);

        const params = {
          reportCode: 'EmployeeHistoryPayroll',
          companyId,
          employeeId,
          DateFrom: this.selectedInitialDate.dateFrom,
          DateTo: this.selectedFinalDate.dateTo
        };

        try {
          const response = await URLBaseAPI.post('/api/Reports/data', params);

          this.companyName = response.data.reportInfo.CompanyName;
          this.employeeName = response.data.reportInfo.EmployeeName;

          this.payrollData = response.data.rows || null;

          this.isReportLoaded = true;

          console.log(this.payrollData);
        } catch (error) {
          const data = error?.response?.data;
          const msg =
            typeof data === 'string' ? data
            : (data && (data.message || data.detail)) || 'Error cargando el detalle de planilla';

          const alert = useGlobalAlert();
          alert.show(msg, "warning");
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
    },
    async mounted() {
      await this.getPayrollPeriodsList();
      await this.loadReport();
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
    background: azure;
    border-radius: 10px;

    display: flex;
    flex-direction: column;
    align-items: flex-start;

    padding: 20px;
    gap: 20px;
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