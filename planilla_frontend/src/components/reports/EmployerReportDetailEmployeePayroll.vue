<template>
  <div id="reportFilters">
    <!-- Empresa -->
     <div>
      <label for="Company" class="form-label fw-bold">Empresa</label>
      <select
        id="Company"
        class="form-select"
        v-model.number="selectedCompanyUniqueId"
        @change="onFiltersChanged"
      >
        <option :value="null">Seleccione una empresa</option>
        <option
          v-for="company in companies"
          :key="company.companyUniqueId"
          :value="company.companyUniqueId"
        >
          {{ company.companyName }}
        </option>
      </select>
     </div>

    <!-- Fecha inicial -->
    <div>
      <label for="StartDate" class="form-label fw-bold">Fecha incial</label>
      <input
        id="StartDate"
        type="date"
        class="form-control"
        v-model="selectedStartDate"
        @change="adjustEndDate"
      />
    </div>

    <!-- Fecha final -->
    <div>
      <label for="EndDate" class="form-label fw-bold">Fecha final</label>
      <input
        id="EndDate"
        type="date"
        class="form-control"
        v-model="selectedEndDate"
        :min="selectedStartDate"
        @change="onFiltersChanged"
      />
    </div>
  </div>

  <div>
    <div id="buttons">
      <LinkButton text="Descargar Excel" @click="downloadExcel()" />
    </div>
  </div>

  <div id="reportContent">
    <h4>Detalle de Planilla Por Empleado</h4>

    <div id="reportTable" class="table-responsive">
      <table class="table">
        <thead>
          <tr>
            <th>Nombre Empleado</th>
            <th>Cedula</th>
            <th>Tipo de Empleado</th>
            <th>Periodo de Pago</th>
            <th>Fecha de Pago</th>
            <th>Salario Bruto</th>
            <th>Cargas Sociales Empleador</th>
            <th>Deducciones Voluntarias</th>
            <th>Costo Empleador</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(row, index) in payrollData" :key="index">
              <td>{{ row.employeeName }}</td>
              <td>{{ row.cedula }}</td>
              <td>{{ row.contractType }}</td>
              <td>{{ row.paymentPeriod }}</td>
              <td>{{ row.paymentDate }}</td>
              <td>{{ row.grossSalary }}</td>
              <td>{{ row.employerDeductions }}</td>
              <td>{{ row.voluntaryDeductions }}</td>
              <td>{{ row.employerCost }}</td>
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
  import URLBaseAPI from '@/axiosAPIInstances'
  import { useSession } from '@/utils/useSession'

  import LinkButton from '../LinkButton.vue';

  const session = useSession()
  const userId = computed(() => session.user?.userId ?? null)
  
  const payrollData = [
    {
      employeeName: 'Pedro Vargas Vargas',
      cedula: '1-5270-0776',
      contractType: 'Tiempo Completp',
      paymentPeriod: '2025-01-10 a 2025-10-31',
      paymentDate: '2025-11-10',
      grossSalary: '₡1,200,000',
      employerDeductions: '₡150,000',
      voluntaryDeductions: '₡50,000',
      employerCost: '₡1,000,000'
    }
  ]

  const companies = ref([])
  const selectedCompanyUniqueId = ref(null)

  const selectedStartDate = ref('')
  const selectedEndDate = ref('')

  function adjustEndDate() {
    if (selectedEndDate.value < selectedStartDate.value) {
      selectedEndDate.value = selectedStartDate.value
    }
  }

  async function loadCompanies() {
    try {
      if (!userId.value) {
        console.warn('userId no disponible aún')
        return
      }

      const resp = await URLBaseAPI.get(`/api/Company/by-user/${userId.value}?onlyActive=false`)

      companies.value = resp.data ?? []
    } catch (err) {
      console.error('Error cargando empresas', err)
    }
  }

  function onFiltersChanged() {
    if (!selectedCompanyUniqueId.value || !selectedStartDate.value || !selectedEndDate.value) {
      // falta back end
      return
    }
  }

  onMounted(() => {
    loadCompanies()
  })

  function downloadExcel() {
    const table = document.getElementById("reportTable").querySelector("table");
    const wb = XLSX.utils.table_to_book(table, { sheet: "Planilla" });
    const wbout = XLSX.write(wb, { bookType: "xlsx", type: "array" });
    const blob = new Blob([wbout], { type: "application/octet-stream" });
    saveAs(blob, "Reporte_Detalle_Planilla_Por_Empleado.xlsx");
  }
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
