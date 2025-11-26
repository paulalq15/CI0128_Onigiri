<template>
  <!-- Restrict page if user is Employee or Admin -->
  <div
    v-if="$session.user?.typeUser === 'Empleado' || $session.user?.typeUser === 'Administrador'"
    class="d-flex flex-column text-center"
  >
    <h3>Acceso restringido</h3>
    <p>Esta página no está disponible</p>
  </div>

  <!-- Everything else must be inside v-else to be the sibling condition -->
  <div v-else class="d-flex flex-column">
    <div class="container mt-5">
      <!-- Employee dropdown: -->
      <div class="mb-4">
        <label for="employeeSelect">Seleccione un empleado:</label>
        <select
          id="employeeSelect"
          v-model="selectedEmployee.id"
          @change="updateSelectedEmployeeInformation"
          class="form-control"
        >
          <option disabled value="">Seleccione un empleado</option>
          <option v-for="(employee, index) in employees" :key="index" :value="employee.idUser">
            {{ 'Id Usuario: ' + employee.idUser + ' ' }}
            {{ employee.name1 }}
            {{ employee.name2 }}
            {{ employee.surname1 }}
            {{ employee.surname2 }}
          </option>
        </select>
      </div>
    </div>

    <div class="container mt-5">
      <div v-if="selectedEmployee.id" class="container mt-5">
        <h1 class="display-6 text-center">Deducciones Disponibles</h1>
        <div class="row justify-content-end">
          <div class="col-2"></div>
        </div>

        <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Tipo de Cálculo</th>
              <th>Valor</th>
              <th>Acción</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="(deduction, index) in filteredDeductions" :key="index">
              <td>{{ deduction.elementName }}</td>
              <td>{{ deduction.calculationType }}</td>
              <td>{{ deduction.calculationValue }}</td>
              <td>
                <button class="btn btn-secondary btn-sm" @click="addAppliedElement(deduction)">
                  Seleccionar
                </button>
              </td>
            </tr>

            <tr>
              <td
                style="
                  text-align: center;
                  width: 50px;
                  height: 50px;
                  border: 1px solid #000;
                  font-weight: bold;
                  vertical-align: middle;
                "
                colspan="6"
              >
                Total de Deducciones: {{ filteredDeductions.length }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>

  <div v-if="selectedEmployee.id" class="container mt-5">
    <h1 class="display-4 text-center">Deducciones Aplicadas</h1>

    <div class="row justify-content-end">
      <div class="col-2"></div>
    </div>

    <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Fecha Inicio</th>
          <th>Fecha Fin</th>
          <th>Estado</th>
          <th>Tipo Plan</th>
          <th>Total Dependientes</th>
          <th>Acción</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="(appliedElement, index) of filteredAppliedElements" :key="index">
          <td>{{ appliedElement.elementName }}</td>
          <td>{{ formatDate(appliedElement.startDate) }}</td>
          <td>{{ formatDate(appliedElement.endDate) }}</td>
          <td>{{ appliedElement.status }}</td>
          <td>{{ appliedElement.planType }}</td>
          <td>{{ appliedElement.amountDependents }}</td>
          <td>
            <button
              class="btn btn-danger btn-sm"
              @click="deactivateAppliedElement(appliedElement.appliedElementId, appliedElement.status)"
            >
              Desactivar
            </button>
          </td>
        </tr>

        <tr>
          <td
            style="
              text-align: center;
              width: 50px;
              height: 50px;
              border: 1px solid #000;
              font-weight: bold;
              vertical-align: middle;
            "
            colspan="7"
          >
            Total Deducciones Activas: {{ this.getTotalActiveAppliedElements() }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import URLBaseAPI from '../axiosAPIInstances.js';
import { getUser } from '../session.js';
import { useGlobalAlert } from '@/utils/alerts.js';

export default {
  name: 'DeductionsList',

  setup() {
    const alert = useGlobalAlert();
    return { alert };
  },

  data() {
    return {
      deductions: [],
      appliedElements: [],
      companyId: null,
      employees: [],
      user: null,

      selectedEmployee: {
        id: null,
        name: '',
      },
    };
  },

  computed: {
    filteredDeductions() {
      return this.deductions.filter((deduction) => deduction.paidBy === 'Empleado');
    },

    deductionElementIds() {
      return new Set(this.filteredDeductions.map((d) => d.idElement));
    },

    filteredAppliedElements() {
      return this.appliedElements.filter(
        (applied) =>
          this.deductionElementIds.has(applied.elementId) && this.isActiveOrEndingThisMonth(applied)
      );
    },
  },

  methods: {
    getEmployees() {
      const user = getUser();

      if (!user) {
        this.alert.show('El usuario actual no está disponible.', 'warning');
        return;
      }

      this.user = user;
      const companyId = user.companyUniqueId;

      URLBaseAPI.get(`/api/PersonUser/getEmployeesByCompanyId?companyId=${companyId}`)
        .then((response) => {
          this.employees = response.data;
        })
        .catch((error) => {
          const msg =
            error?.response?.status === 404
              ? 'No se encontraron empleados para la empresa.'
              : 'Error al obtener los empleados.';
          this.alert.show(msg, 'warning');
        });
    },

    async getCompanyIdByUserId() {
      try {
        const response = await URLBaseAPI.get(
          `/api/Company/getCompanyIdByUserId?userId=${this.user.userId}`
        );
        this.companyId = response.data;
        this.getDeductions();
      } catch (error) {
        const msg =
          error?.response?.status === 404
            ? 'No se encontró la empresa del usuario.'
            : 'Error al obtener la empresa del usuario.';
        this.alert.show(msg, 'warning');
      }
    },

    getDeductions() {
      if (!this.companyId) {
        this.alert.show('Aún no se ha definido la empresa del usuario.', 'warning');
        return;
      }

      URLBaseAPI.get(`/api/PayrollElement/GetPayRollElements`, {
        params: {
          idCompany: this.companyId,
        },
      })

        .then((response) => {
          this.deductions = response.data;
        })

        .catch(() => {
          this.alert.show('Error al obtener las deducciones.', 'warning');
        });
    },

    getAppliedElements() {
      URLBaseAPI.get(
        `/api/AppliedElement/getAppliedElements?employeeId=${this.selectedEmployee.id}`
      )
        .then((response) => {
          this.appliedElements = response.data;
        })

        .catch(() => {
          this.alert.show('Error al obtener las deducciones aplicadas.', 'warning');
        });
    },

    formatDate(dateString) {
      if (dateString == null) {
        return '-';
      }

      const date = new Date(dateString);
      return date.toLocaleDateString();
    },

    getTotalActiveAppliedElements() {
      return this.filteredAppliedElements.filter((element) => element.status === 'Activo').length;
    },

    addAppliedElement(deduction) {
      // Verify the parameters sent to the function:
      if (!this.user || !this.user.userId) {
        this.alert.show('El usuario actual no está definido.', 'warning');
        return;
      }

      if (!deduction.idElement) {
        this.alert.show('La deducción no tiene un identificador válido.', 'warning');
        return;
      }

      // Verify that an employee was selected:
      if (!this.selectedEmployee || !this.selectedEmployee.id) {
        this.alert.show(
          'Por favor, seleccione un empleado antes de seleccionar una deducción.',
          'warning'
        );
        return;
      }

      // Verify that the benefit hasn't been selected yet:
      const alreadySelected = this.appliedElements.some(
        (applied) => applied.elementName == deduction.elementName && applied.status == 'Activo'
      );

      if (alreadySelected) {
        this.alert.show('Esta deducción ya está seleccionada.', 'warning');
        return;
      }

      // Make a POST request to add the new applied element:
      URLBaseAPI.post(`/api/AppliedElement/addAppliedElement`, {
        UserId: this.selectedEmployee.id,
        ElementId: deduction.idElement,
        ElementType: 'Deduccion',
        AmountDependents: null,
        PlanType: null,
      })

        .then(() => {
          this.alert.show('Deducción agregada exitosamente.', 'warning');

          // Update the appliedElements list and totals after the successful addition:
          this.getAppliedElements();
        })

        .catch(() => {
          this.alert.show('Error al agregar la deducción.', 'warning');
        });
    },

    async deactivateAppliedElement(appliedElementId, status) {
      if (status == 'Inactivo') {
        this.alert.show('El elemento seleccionado ya está inactivo.', 'warning');
        return;
      }

      try {
        await URLBaseAPI.post('/api/AppliedElement/deactivateAppliedElement', {
          appliedElementId: appliedElementId,
        });

        this.alert.show('Deducción desactivada correctamente.', 'warning');
        this.getAppliedElements();
      } catch {
        this.alert.show('Error al desactivar la deducción.', 'warning');
      }
    },

    isActiveOrEndingThisMonth(el) {
      if (el.status === 'Activo') return true;
      if (el.status !== 'Inactivo') return false;
      if (!el.endDate) return false;

      const end = new Date(el.endDate);
      if (Number.isNaN(end.getTime())) {
        return false;
      }

      const now = new Date();
      const sameYear = end.getFullYear() === now.getFullYear();
      const sameMonth = end.getMonth() === now.getMonth();

      return sameYear && sameMonth;
    },

    updateSelectedEmployeeInformation() {
      const employee = this.employees.find((e) => e.idUser === this.selectedEmployee.id);

      if (employee) {
        // Update selectedEmployee name:
        this.selectedEmployee.name =
          employee.name1 +
          ' ' +
          (employee.name2 || '') +
          ' ' +
          employee.surname1 +
          ' ' +
          (employee.surname2 || '');

        // Fetch applied elements for selected employee:
        this.getAppliedElements();
      }
    },
  },

  created() {
    this.user = getUser();
    this.getCompanyIdByUserId();
    this.getEmployees();
  },
};
</script>

<style lang="scss" scoped></style>
