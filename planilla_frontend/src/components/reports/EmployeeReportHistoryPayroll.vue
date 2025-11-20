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

<script setup>
  import { ref, computed, onMounted } from 'vue';
  import * as XLSX from "xlsx";
  import { saveAs } from "file-saver";
  import { useSession } from '@/utils/useSession';
  import { useGlobalAlert } from '@/utils/alerts.js';

  import URLBaseAPI from '../../axiosAPIInstances.js';
  import LinkButton from '../LinkButton.vue';

  const { user } = useSession();

  const companyName = ref("");
  const employeeName = ref("");
  
  const initialDatesList = ref([]);
  const selectedInitialDate = ref(null);
  
  const waitingMessage = ref("Cargando datos...");

  const finalDatesList = computed(() => {
    if (!selectedInitialDate.value) return [];

    const index = initialDatesList.value.findIndex(
      item => item.payrollId === selectedInitialDate.value.payrollId
    );

    if (index === -1) return [];

    return initialDatesList.value.slice(index);
  });
  const selectedFinalDate = ref(null);
  
  const payrollData = ref([]);

  const isReportLoaded = ref(false);

  async function getPayrollPeriodsList() {
    const companyId = user?.companyUniqueId;
    const employeeId = Number(user?.personId);

    try {
      const response = await URLBaseAPI.get('/api/Reports/employee/payroll-periods', {
        params: { companyId, employeeId }
      });

      initialDatesList.value = response.data || [];

      if (initialDatesList.value.length > 0) {
        selectedInitialDate.value = initialDatesList.value[0];
        selectedFinalDate.value = initialDatesList.value[initialDatesList.value.length - 1];
      }
    } catch (error) {
      const data = error?.response?.data;
      const msg =
        typeof data === "string" ? data
        : data?.message || data?.detail || "Error cargando las últimas planillas";

      const alert = useGlobalAlert();
      alert.show(msg, "warning");
    }
  }

  async function loadReport() {
    if (!selectedInitialDate.value  || !selectedFinalDate.value) {
      waitingMessage.value = "No hay datos que mostrar";
      return;
    }

    const companyId = user?.companyUniqueId;
    const employeeId = Number(user?.personId);

    const params = {
      reportCode: 'EmployeeHistoryPayroll',
      companyId,
      employeeId,
      DateFrom: selectedInitialDate.value.dateFrom,
      DateTo: selectedFinalDate.value.dateTo
    };

    try {
      const response = await URLBaseAPI.post('/api/Reports/data', params);

      companyName.value = response.data.reportInfo.CompanyName;
      employeeName.value = response.data.reportInfo.EmployeeName;

      payrollData.value = response.data.rows || null;

      isReportLoaded.value = true;

      console.log(payrollData.value);
    } catch (error) {
      const data = error?.response?.data;
      const msg =
        typeof data === 'string' ? data
        : (data && (data.message || data.detail)) || 'Error cargando el detalle de planilla';

      const alert = useGlobalAlert();
      alert.show(msg, "warning");
    }
  }

  function downloadExcel() {
    const table = document.getElementById("reportTable").querySelector("table");
    const wb = XLSX.utils.table_to_book(table, { sheet: "Planilla" });
    const wbout = XLSX.write(wb, { bookType: "xlsx", type: "array" });
    const blob = new Blob([wbout], { type: "application/octet-stream" });
    saveAs(blob, "Reporte_Planilla.xlsx");
  }

  onMounted(async () => {
    await getPayrollPeriodsList();
    await loadReport();
  });

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