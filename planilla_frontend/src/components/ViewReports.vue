<template>
  <div class="container py-5">
    <h1 class="display-5 text-center">
      {{ selectedReport ? `Reporte - ${selectedReport.description}` : 'Reporte' }}
    </h1>

    <!-- Select for reports -->
    <div class="w-50 mb-4">
      <label for="selectReports" class="form-label fw-bold">Reportes</label>
      <select id="selectReports" class="form-select" v-model="selectedReportId">
        <option disabled value="">Seleccione un tipo de reporte</option>
        <option
          v-for="report in reports"
          :key="report.id"
          :value="report.id"
        >
          {{ report.description }}
        </option>
      </select>
    </div>

    <!-- Div container of report-->
    <div class="text-center">
      <div v-if="!selectedReportId" class="text-muted mb-2">
        No hay un reporte seleccionado
      </div>
      <component v-else :is="selectedReport.component"></component>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed } from 'vue'
  import { useSession } from '@/utils/useSession';
  import { getReportsByUserType } from './reports/reportsConfig';
  
  const { user } = useSession();
  
  // Reportes según el tipo de usuario
  const reports = getReportsByUserType(user.typeUser);

  const selectedReportId = ref('');

  const selectedReport  = computed(() => {
    return reports.find(r => r.id === selectedReportId.value);
  });
</script>

<style lang="scss" scoped>
  
</style>

<style lang="scss" scoped>
  
</style>