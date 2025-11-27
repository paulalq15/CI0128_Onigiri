<template>
  <div class="container py-4">
    <h3 class="mb-4">Dashboard de empleado</h3>

    <!-- Bar chart comparativo salario bruto y neto -->
    <div class="row">
      <div class="col-12 col-lg-6">
        <div class="card">
          <div class="card-header fw-bold">
            Gráfico comparativo entre salario bruto y neto
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
import { useGlobalAlert } from '@/utils/alerts.js'

export default {
  name: 'EmployeeDashboard',

  setup() {
    const session = useSession()
    const employeeId = session.user?.personId ?? 0
    const alert = useGlobalAlert()

    const employeeFiguresPerMonth = ref([])

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
      if (!employeeId) {
        alert.show('No se encontró el usuario en la sesión', 'warning')
        return
      }

      const { data } = await URLBaseAPI.get(`/api/Dashboard/employee/${employeeId}`)
      employeeFiguresPerMonth.value = data.employeeFiguresPerMonth || []

      barOptions.value = {
        ...barOptions.value,
        xaxis: {
          categories: ['Último mes de pago']
        }
      }

      barSeries.value = [
      {
        name: 'Salario Bruto',
        data: employeeFiguresPerMonth.value.map(x => x.grossSalary)
      },

      {
        name: 'Salario Neto',
        data: employeeFiguresPerMonth.value.map(x => x.netSalary)
      }]
    } 

    catch (error) {
      alert.show('Error cargando el dashboard de empleado: ' + error, 'warning')
    }
  })

    return {
      employeeFiguresPerMonth,
      barSeries,
      barOptions
    }
  }
}

</script>

<style lang="scss" scoped></style>
