<template>
  <div>
    <div v-if="companiesError" class="alert alert-danger mb-3">
      {{ companiesError }}
    </div>

    <div id="reportFilters">
      <!-- Empresa -->
      <div>
        <label for="Company" class="form-label fw-bold">Empresa</label>
        <select
          id="Company"
          class="form-select"
          v-model="selectedCompanyUniqueId"
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

      <!-- Tipo Empleado -->
      <div>
        <label for="EmployeeType" class="form-label fw-bold">Tipo de Empleado</label>
        <select
          id="EmployeeType"
          class="form-select"
          v-model="selectedEmployeeType"
          @change="onFiltersChanged"
        >
          <option :value="null">Seleccione un tipo</option>
          <option value="FullTime">Tiempo Completo</option>
          <option value="PartTime">Medio Tiempo</option>
          <option value="ProfessionalServices">Servicios Profesionales</option>
        </select>
      </div>

      <!-- Cedula Empleado -->
      <div>
        <label for="EmployeeNationalId" class="form-label fw-bold">Cédula de Empleado</label>
        <input
          id="EmployeeNationalId"
          type="text"
          class="form-control"
          v-model="selectedEmployeeNationalId"
          @input="formatNationalId"
          @change="onFiltersChanged"
          pattern="\\d{1}-\\d{4}-\\d{4}"
          placeholder="1-2345-6789"
        />
        <p v-if="cedulaError" class="text-danger small mt-1">
          {{ cedulaError }}
        </p>
      </div>

      <!-- Fecha inicial -->
      <div>
        <label for="StartDate" class="form-label fw-bold">Fecha inicial</label>
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
      <div id="buttons" class="mt-3">
        <LinkButton text="Descargar Excel" @click="downloadExcel()" />
      </div>
    </div>

    <div id="reportContent" class="mt-4">
      <h4>Detalle de Planilla Por Empleado</h4>

      <div v-if="reportError" class="alert alert-warning mb-3">
        {{ reportError }}
      </div>

      <div id="reportTable" class="table-responsive">
        <table class="table">
          <thead>
            <tr>
              <th>Nombre Empleado</th>
              <th>Cédula</th>
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
              <td>{{ row.EmployeeName }}</td>
              <td>{{ row.NationalId }}</td>
              <td>{{ row.EmployeeType }}</td>
              <td>{{ row.PaymentPeriod }}</td>
              <td>{{ row.PaymentDate }}</td>
              <td>{{ row.GrossSalary }}</td>
              <td>{{ row.EmployerContributions }}</td>
              <td>{{ row.EmployeeBenefits }}</td>
              <td>{{ row.EmployerCost }}</td>
            </tr>
            <tr v-if="!payrollData.length && !reportError">
              <td colspan="9" class="text-center text-muted">
                No hay datos para mostrar.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
  import * as XLSX from "xlsx";
  import { saveAs } from "file-saver";
  import URLBaseAPI from '@/axiosAPIInstances'
  import { useSession } from '@/utils/useSession'

  import LinkButton from '../LinkButton.vue';

  export default {
    components: {
      LinkButton,
    },
    data() {
      return {
        session: useSession(),

        payrollData: [],

        companies: [],
        selectedCompanyUniqueId: null,

        selectedStartDate: '',
        selectedEndDate: '',

        selectedEmployeeType: null,
        selectedEmployeeNationalId: '',

        companiesError: '',
        reportError: '',
        cedulaError: '',
      }
    },
    computed: {
      userId() {
        return this.session.user?.userId ?? null
      }
    },
    methods: {
      adjustEndDate() {
        if (this.selectedEndDate < this.selectedStartDate) {
          this.selectedEndDate = this.selectedStartDate
        }
      },

      formatNationalId(event) {
        let raw = event.target.value.replace(/\D/g, "");

        if (raw.length > 9) {
          raw = raw.slice(0, 9);
        }

        let formatted = raw;

        if (raw.length > 1) {
          formatted = raw[0] + "-" + raw.slice(1);
        }
        if (raw.length > 5) {
          formatted = raw[0] + "-" + raw.slice(1, 5) + "-" + raw.slice(5);
        }

        this.selectedEmployeeNationalId = formatted;
      },

      async loadCompanies() {
        this.companiesError = ''
        try {
          if (!this.userId) {
            this.companiesError = 'No se pudo obtener la información del usuario. Intente iniciar sesión nuevamente.'
            return
          }

          const resp = await URLBaseAPI.get(`/api/Company/by-user/${this.userId}?onlyActive=false`)

          this.companies = resp.data ?? []

          if (!this.companies.length) {
            this.companiesError = 'No se encontraron empresas asociadas a tu usuario.'
          }

        } catch (err) {
          this.companiesError = 'Error cargando empresas. Intenta nuevamente más tarde.'
        }
      },

      async onFiltersChanged() {
        this.reportError = ''
        this.cedulaError = ''

        if (!this.selectedCompanyUniqueId || !this.selectedStartDate || !this.selectedEndDate) {
          this.reportError = 'Selecciona la empresa y el rango de fechas para cargar el reporte.'
          return;
        }

        if (this.selectedEmployeeNationalId) {
          const cedulaRegex = /^\d-\d{4}-\d{4}$/;

          if (!cedulaRegex.test(this.selectedEmployeeNationalId)) {
            this.cedulaError = 'La cédula debe tener el formato #-####-####.'
            return;
          }
        }

        const params = {
          reportCode: 'EmployerEmployeePayroll',
          companyId: this.selectedCompanyUniqueId,
          employeeNationalId: this.selectedEmployeeNationalId || null,
          employeeType: this.selectedEmployeeType ?? null,
          dateFrom: this.selectedStartDate,
          dateTo: this.selectedEndDate,
        }

        try {
          const { data } = await URLBaseAPI.post('/api/Reports/data', params);
          const rows = Array.isArray(data.rows) ? data.rows : [];
          this.payrollData = rows;

          if (!rows.length) {
            this.reportError = 'No se encontraron datos para los filtros seleccionados.'
          }

        } catch (error) {
          const backendMessage = error?.response?.data?.message;
          this.reportError = backendMessage || 'No se pudo cargar el reporte. Intenta nuevamente más tarde.';
        }
      },

      downloadExcel() {
        const table = document.getElementById("reportTable").querySelector("table");
        const wb = XLSX.utils.table_to_book(table, { sheet: "Planilla" });
        const wbout = XLSX.write(wb, { bookType: "xlsx", type: "array" });
        const blob = new Blob([wbout], { type: "application/octet-stream" });
        saveAs(blob, "Reporte_Detalle_Planilla_Por_Empleado.xlsx");
      }
    },

    mounted() {
      this.loadCompanies()
    }
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
