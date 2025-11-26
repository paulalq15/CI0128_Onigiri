<template>
  <div class="container py-4">
    <h3 class="mb-4">Dashboard de Costos y Empleados</h3>

    <!-- Pie chart Costos por tipo -->
    <div class="row mb-4">
      <div class="col-12 col-lg-6">
        <div class="card">
          <div class="card-header fw-bold">
            Costos por tipo
          </div>
          <div class="card-body">
            <apexchart
              v-if="pieSeries.length > 0"
              type="pie"
              height="300"
              :options="pieOptions"
              :series="pieSeries"
            />
            <p v-else class="text-muted mb-0">No hay datos de costos por tipo.</p>
          </div>
        </div>
      </div>

      <!-- Line chart Costos por mes -->
      <div class="col-12 col-lg-6 mt-4 mt-lg-0">
        <div class="card">
          <div class="card-header fw-bold">
            Tendencia de costos por mes
          </div>
          <div class="card-body">
            <apexchart
              v-if="lineSeries[0].data.length > 0"
              type="line"
              height="300"
              :options="lineOptions"
              :series="lineSeries"
            />
            <p v-else class="text-muted mb-0">No hay datos de costos por mes.</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Bar chart Empleados por tipo -->
    <div class="row">
      <div class="col-12 col-lg-6">
        <div class="card">
          <div class="card-header fw-bold">
            Empleados por tipo
          </div>
          <div class="card-body">
            <apexchart
              v-if="barSeries[0].data.length > 0"
              type="bar"
              height="300"
              :options="barOptions"
              :series="barSeries"
            />
            <p v-else class="text-muted mb-0">No hay datos de empleados por tipo.</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import URLBaseAPI from '@/axiosAPIInstances'
import { useSession } from '@/utils/useSession'
import { useGlobalAlert } from '@/utils/alerts.js';

export default {
  name: 'EmployerDashboard',

  setup() {
    const session = useSession()
    const companyId = session.user?.companyUniqueId ?? 0
    const alert = useGlobalAlert()

    const costByTypes = ref([])
    const costByMonth = ref([])
    const employeeCountByType = ref([])

    // --- Pie chart ---
    const pieSeries = ref([])
    const pieOptions = ref({
      chart: {
        background: '#ffffff'
      },
      labels: [],
      legend: { position: 'bottom' }
    })

    // --- Line chart ---
    const lineSeries = ref([{ name: 'Costo total', data: [] }])
    const lineOptions = ref({
      chart: {
        background: '#ffffff'
      },
      xaxis: { categories: [] },
      stroke: { curve: 'smooth' }
    })

    // --- Bar chart ---
    const barSeries = ref([{ name: 'Empleados', data: [] }])
    const barOptions = ref({
      chart: {
        background: '#ffffff'
      },
      xaxis: { categories: [] },
      plotOptions: {
        bar: { borderRadius: 4, columnWidth: '50%' }
      }
    })

    onMounted(async () => {
      try {
        if (!companyId) {
          alert.show('No se encontró compañia en la sesión', 'warning')
          return
        }

        const { data } = await URLBaseAPI.get(`/api/Dashboard/employer/${companyId}`)

        // Guardar datos crudos:
        costByTypes.value = data.costByTypes || []
        costByMonth.value = data.costByMonth || []
        employeeCountByType.value = data.employeeCountByType || []

        // ----- Pie chart -----
        pieOptions.value = {
          ...pieOptions.value,
          labels: costByTypes.value.map(x => x.costType)
        }
        pieSeries.value = costByTypes.value.map(x => x.totalCost)

        // ----- Line chart -----
        lineOptions.value = {
          ...lineOptions.value,
          xaxis: { categories: costByMonth.value.map(x => x.month) }
        }
        lineSeries.value = [
          {
            name: 'Costo total',
            data: costByMonth.value.map(x => x.totalCost)
          }
        ]

        // ----- Bar chart -----
        barOptions.value = {
          ...barOptions.value,
          xaxis: { categories: employeeCountByType.value.map(x => x.employeeType) }
        }
        barSeries.value = [
          {
            name: 'Empleados',
            data: employeeCountByType.value.map(x => x.employeeCount)
          }
        ]
      } catch (error) {
        alert.show('Error cargando dashboard: ' + error, 'warning')
      }
    })

    return {
      // datos
      costByTypes,
      costByMonth,
      employeeCountByType,

      // charts
      pieSeries,
      pieOptions,
      lineSeries,
      lineOptions,
      barSeries,
      barOptions
    }
  }
}
</script>

<style lang="scss" scoped>

</style>