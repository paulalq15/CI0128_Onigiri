<template>
  <div id="reportFilters">
    <!-- Fecha inicial -->
    <div>
      <label for="selectFechaInicial" class="form-label fw-bold">Fecha incial</label>
      <select id="selectFechaInicial" class="form-select" v-model="selectedInitialDate">
        <option
          v-for="date in initialDatesList"
          :key="date"
          :value="date"
        >
          {{ date }}
        </option>
      </select>
    </div>

    <!-- Fecha final -->
    <div>
      <label for="selectFechaFinal" class="form-label fw-bold">Fecha final</label>
      <select id="selectFechaFinal" class="form-select" v-model="selectedFinalDate">
        <option
          v-for="date in finalDatesList"
          :key="date"
          :value="date"
        >
          {{ date }}
        </option>
      </select>
    </div>
  </div>

  <div>
    <div id="buttons">
      <LinkButton text="Descargar Excel" @click="downloadExcel()" />
    </div>
  </div>

  <div id="reportContent">
    <h4>Reporte histórico pago de planilla</h4>
    
    <p>Empresa: {{ companyName }}</p>
    <p>Nombre del empleado: {{ employeeName }}</p>

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
          <tr v-for="(row, index) in payrollData" :key="index">
              <td>{{ row.contractType }}</td>
              <td>{{ row.position }}</td>
              <td>{{ row.paymentDate }}</td>
              <td>{{ row.grossSalary }}</td>
              <td>{{ row.mandatoryDeductions }}</td>
              <td>{{ row.voluntaryDeductions }}</td>
              <td>{{ row.netSalary }}</td>
            </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed } from 'vue';
  import * as XLSX from "xlsx";
  import { saveAs } from "file-saver";

  import LinkButton from '../LinkButton.vue';

  const companyName = ref("ABC");
  const employeeName = ref("Josué Badilla");
  
  const initialDatesList = ref(["09/2025", "10/2025", "11/2025", "12/2025"]);
  const selectedInitialDate = ref("09/2025");
  
  const finalDatesList = computed(() => {
    // Obtenemos el inicio para la segunda lista desde la seleccionada en fecha inicial
    const index = initialDatesList.value.indexOf(selectedInitialDate.value);
    if (index === -1) return [];
    return initialDatesList.value.slice(index);
  });
  const selectedFinalDate = ref("12/2025");

  const payrollData = [
    {
      contractType: 'Indefinido',
      position: 'Desarrollador',
      paymentDate: '2025-11-10',
      grossSalary: '₡1,200,000',
      mandatoryDeductions: '₡150,000',
      voluntaryDeductions: '₡50,000',
      netSalary: '₡1,000,000'
    }
  ]

  function downloadExcel() {
    const table = document.getElementById("reportTable").querySelector("table");
    const wb = XLSX.utils.table_to_book(table, { sheet: "Planilla" });
    const wbout = XLSX.write(wb, { bookType: "xlsx", type: "array" });
    const blob = new Blob([wbout], { type: "application/octet-stream" });
    saveAs(blob, "Reporte_Planilla.xlsx");
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