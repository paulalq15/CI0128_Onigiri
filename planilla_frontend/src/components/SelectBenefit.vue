<template>
  <div v-if="$session.user?.typeUser === 'Administrador'" class="d-flex flex-column text-center">
    <h3>Acceso restringido</h3>
    <p>Esta página no está disponible</p>
  </div>

  <div v-else-if="$session.user?.typeUser === 'Empleado'" class="d-flex flex-column">
    <div class="container mt-5">
      <h1 class="display-4 text-center">Beneficios Disponibles</h1>

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
          <tr v-for="(benefit, index) of filteredBenefits" :key="index">
            <td>{{ benefit.elementName }}</td>
            <td>{{ benefit.calculationType }}</td>
            <td>{{ benefit.calculationValue }}</td>
            <td>
              <button class="btn btn-secondary btn-sm" @click="addAppliedElement(benefit)">
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
            >
              Total de Beneficios: {{ filteredBenefits.length }}
            </td>

            <td
              style="
                text-align: center;
                width: 50px;
                height: 50px;
                border: 1px solid #000;
                font-weight: bold;
                vertical-align: middle;
              "
            >
              Máx. beneficios ofrecidos por la empresa: {{ this.maxCompanyBenefits }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="container mt-5">
      <h1 class="display-4 text-center">Beneficios Aplicados</h1>

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
          <tr v-for="(appliedElement, index) of this.filterAppliedElements" :key="index">
            <td>{{ appliedElement.elementName }}</td>
            <td>{{ this.formatDate(appliedElement.startDate) }}</td>
            <td>{{ this.formatDate(appliedElement.endDate) }}</td>
            <td>{{ appliedElement.status }}</td>
            <td>{{ appliedElement.planType }}</td>
            <td>{{ appliedElement.amountDependents }}</td>
            <td>
              <button
                class="btn btn-danger btn-sm"
                @click="
                  this.deactivateAppliedElement(appliedElement.elementId, appliedElement.status)
                "
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
            >
              Beneficios activos: {{ this.getTotalActiveAppliedBenefits() }}
            </td>

            <td
              style="
                text-align: center;
                width: 50px;
                height: 50px;
                border: 1px solid #000;
                font-weight: bold;
                vertical-align: middle;
              "
            >
              Beneficios restantes: {{ maxCompanyBenefits - this.getTotalActiveAppliedBenefits() }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>

  <!-- Message for other type of user trying to access this page: -->
  <div v-else class="d-flex flex-column text-center">
    <p>Acceso no autorizado</p>
  </div>
</template>

<script>
import URLBaseAPI from '../axiosAPIInstances.js';
import { getUser } from '../session.js';

export default {
  name: 'BenefitsList',

  data() {
    return {
      benefits: [],
      appliedElements: [],
      filteredAppliedElements: [],
      totalCompanyBenefits: 0,
      maxCompanyBenefits: 0,
      companyId: null,
      user: null,
    };
  },

  computed: {
    filteredBenefits() {
      return this.benefits.filter((benefit) => benefit.paidBy === 'Empleador');
    },

    benefitElementIds() {
      return new Set(this.filteredBenefits.map((d) => d.idElement));
    },

    filterAppliedElements() {
      return this.appliedElements.filter((applied) => this.isActiveOrEndingThisMonth(applied));
    },
  },

  methods: {
    async getCompanyIdByUserId() {
      try {
        const response = await URLBaseAPI.get(
          `/api/Company/getCompanyIdByUserId?userId=${this.user.userId}`
        );
        this.companyId = response.data;
        this.getBenefits();
        this.maxCompanyBenefits = this.getCompanyTotalBenefitsElements();
      } catch (error) {
        console.error('Error getting companyId:', error);
      }
    },

    getBenefits() {
      if (!this.companyId) {
        console.warn('CompanyId is not set yet');
        return;
      }

      URLBaseAPI.get(`/api/PayrollElement/GetPayRollElements`, {
        params: {
          idCompany: this.companyId,
        },
      })

        .then((response) => {
          this.benefits = response.data;
        })

        .catch((error) => {
          console.error('Error fetching benefits:', error);
        });
    },

    getCompanyTotalBenefitsElements() {
      URLBaseAPI.get(`/api/Company/getCompanyTotalBenefitsByCompanyId?CompanyId=${this.companyId}`)
        .then((response) => {
          this.maxCompanyBenefits = response.data;
        })

        .catch((error) => {
          console.error('Error getting applied elements:', error);
        });
    },

    getAppliedElements() {
      URLBaseAPI.get(`/api/AppliedElement/getAppliedElements?employeeId=${this.user.userId}`)
        .then((response) => {
          this.appliedElements = response.data;
        })

        .catch((error) => {
          console.error('Error getting applied elements:', error);
        });
    },

    formatDate(dateString) {
      if (dateString == null) {
        return '-';
      }

      const date = new Date(dateString);
      return date.toLocaleDateString();
    },

    getTotalActiveAppliedBenefits() {
      //const benefitIds = new Set(this.filteredBenefits.map((b) => b.idElement));
      return this.appliedElements.filter((el) => this.isActiveOrEndingThisMonth(el)).length;
    },

    getTotalActiveBenefits() {
      return this.filteredBenefits.filter((element) => element.status === 'Activo').length;
    },

    addAppliedElement(benefit) {
      // Verify if the employee reached the max ammount of benefits:
      if (this.maxCompanyBenefits - this.getTotalActiveAppliedBenefits() == 0) {
        alert('ALERTA: Se llegó al máximo de beneficios activos disponibles.');
        return;
      }

      // Verify that the benefit hasn't been selected yet:
      const alreadySelected = this.appliedElements.some(
        (applied) => applied.elementName == benefit.elementName && applied.status == 'Activo'
      );

      if (alreadySelected) {
        alert('Este beneficio ya está seleccionado.');
        return;
      }

      if (!this.user || !this.user.userId) {
        alert('User ID no está definido');
        return;
      }

      if (!benefit.idElement) {
        alert('ERROR: benefit.idElement no está definido.');
        return;
      }

      let amountDependents = null;
      let planType = null;

      // Verify if the deduction is an API:
      if (benefit.calculationType === 'API') {
        // Seguro Privado:
        if (benefit.calculationValue === 2) {
          amountDependents = prompt('Ingrese la cantidad de dependientes: ');

          // if (!Number.isInteger(amountDependents)) {
          if (amountDependents === parseInt(amountDependents, 10)) {
            alert('ERROR: la cantidad de dependientes debe ser un número entero.');
            return;
          }

          if (amountDependents <= 0) {
            alert('ERROR: la cantidad de dependientes debe ser superior a 0.');
            return;
          }
        }

        // Pensión Voluntaria:
        if (benefit.calculationValue === 3) {
          planType = prompt('Ingrese el tipo de plan:');

          if (planType != 'A' && planType != 'B' && planType != 'C') {
            alert('ERROR: el tipo de plan debe ser A, B o C.');
            return;
          }
        }
      }

      // Make a POST request to add the new applied element:
      URLBaseAPI.post('/api/AppliedElement/addAppliedElement', {
        UserId: this.user.userId,
        ElementId: benefit.idElement,
        ElementType: 'Beneficio',
        AmountDependents: amountDependents,
        PlanType: planType,
      })

        .then((response) => {
          alert('Beneficio agregado exitosamente.');

          // Update the appliedElements list and totals after the successful addition:
          this.appliedElements.push(response.data);
          window.location.reload();
        })

        .catch((error) => {
          console.error('Error adding applied element:', error);
          alert('Error al agregar el beneficio.');
        });
    },

    async deactivateAppliedElement(appliedElementId, status) {
      if (status == 'Inactivo') {
        alert('El elemento seleccionado ya está inactivo.');
        return;
      }

      try {
        await URLBaseAPI.post('/api/AppliedElement/deactivateAppliedElement', {
          ElementId: appliedElementId,
        });

        window.location.reload();
      } catch (error) {
        if (error.code === 'ECONNABORTED') {
          console.error('Request aborted or timed out');
        } else {
          console.error('Error in request:', error.message);
        }
      }
    },

    isActiveOrEndingThisMonth(el) {

      // Si está activo, siempre cuenta
      if (el.status === 'Activo') return true;

      // Si no es inactivo, no cuenta
      if (el.status !== 'Inactivo') return false;

      // Si es inactivo, revisamos endDate
      if (!el.endDate) return false;

      const end = new Date(el.endDate);
      if (isNaN(end.getTime())) {
        console.warn('endDate inválido en AppliedElement:', el.endDate, el);
        return false;
      }

      const now = new Date();
      const sameYear = end.getFullYear() === now.getFullYear();
      const sameMonth = end.getMonth() === now.getMonth(); // 0–11

      return sameYear && sameMonth;
    },

  },

  created() {
    this.user = getUser();
    this.getCompanyIdByUserId();
    this.getAppliedElements();
  },
};
</script>

<style lang="scss" scoped></style>
