<template>
  <div class="container py-5">
    <h1 class="display-5 text-center">
      {{ selectedReport ? `Reporte - ${reportTitle}` : 'Reporte' }}
    </h1>

    <!-- Select for reports -->
    <div class="w-50 mb-4">
      <label for="selectReports" class="form-label fw-bold">Reportes</label>
      <select id="selectReports" class="form-select" v-model="selectedReport">
        <option disabled value="">Seleccione un tipo de reporte</option>
        <option
          v-for="reportType in reportsList"
          :key="reportType.id"
          :value="reportType.id"
        >
          {{ reportType.description }}
        </option>
      </select>
    </div>

    <!-- Div container of report-->
    <div class="text-center">
      <div v-if="!selectedReport" class="text-muted mb-2">
        No hay un reporte seleccionado
      </div>
      <component :is="selectedComponent"></component>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed } from 'vue'

  import EmployeeReportDetailPayroll from './EmployeeReportDetailPayroll.vue';
  import EmployeeReportHistoryPayroll from './EmployeeReportHistoryPayroll.vue';

  const reportsList = [
    {id: 1, description: "Detalle pago de planilla"},
    {id: 2, description: "Histórico pago de planilla"}
  ];

  const  selectedReport = ref("");

  const reportTitle = computed(() => {
    if (!selectedReport.value) return "";

    const temp = reportsList.find(r => r.id === selectedReport.value);
    return temp ? temp.description : "";
  });

  const selectedComponent = computed(() => {
    switch (selectedReport.value) {
      case 1:
        return EmployeeReportDetailPayroll;
      case 2:
        return EmployeeReportHistoryPayroll;
      default:
        return null;
    }
  });
</script>

<style lang="scss" scoped>
  
</style>