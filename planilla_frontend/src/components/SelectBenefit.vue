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
                  this.deactivateAppliedElement(appliedElement.appliedElementId, appliedElement.status)
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

    <div
      v-if="showExtraDataModal"
      class="modal fade show d-block"
      tabindex="-1"
      style="background-color: rgba(0, 0, 0, 0.4);"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Datos adicionales del beneficio</h5>
            <button type="button" class="btn-close" @click="cancelExtraData"></button>
          </div>

          <div class="modal-body">
            <p class="mb-3">
              {{ pendingBenefit ? pendingBenefit.elementName : '' }}
            </p>

            <div v-if="extraFieldType === 'dependents'">
              <label class="form-label">Cantidad de dependientes</label>
              <input
                type="number"
                min="1"
                class="form-control"
                v-model.number="tempAmountDependents"
              />
              <small class="text-muted">Debe ser un número entero mayor que cero.</small>
            </div>

            <div v-else-if="extraFieldType === 'plan'">
              <label class="form-label">Tipo de plan</label>
              <select class="form-select" v-model="tempPlanType">
                <option value="">Seleccione una opción</option>
                <option value="A">Plan A</option>
                <option value="B">Plan B</option>
                <option value="C">Plan C</option>
              </select>
              <small class="text-muted">Solo se permiten planes A, B o C.</small>
            </div>
          </div>

          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="cancelExtraData">
              Cancelar
            </button>
            <button type="button" class="btn btn-primary" @click="confirmExtraData">
              Confirmar
            </button>
          </div>
        </div>
      </div>
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
import { useGlobalAlert } from '@/utils/alerts.js';

export default {
  name: 'BenefitsList',
  setup() {
    const alert = useGlobalAlert();
    return { alert };
  },
  data() {
    return {
      benefits: [],
      appliedElements: [],
      filteredAppliedElements: [],
      totalCompanyBenefits: 0,
      maxCompanyBenefits: 0,
      companyId: null,
      user: null,

      showExtraDataModal: false,
      pendingBenefit: null,
      extraFieldType: null,
      tempAmountDependents: null,
      tempPlanType: '',
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
      return this.appliedElements.filter(
        (applied) =>
          this.benefitElementIds.has(applied.elementId) &&
          this.isActiveOrEndingThisMonth(applied)
      );
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
        this.getCompanyTotalBenefitsElements();
      } catch (error) {
        const isNotFound = error?.response?.status === 404;
        const msg = isNotFound
          ? 'No se encontró la empresa del usuario.'
          : 'Error al obtener la empresa del usuario.';
        this.alert.show(msg, 'warning');
      }
    },

    getBenefits() {
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
          this.benefits = response.data;
        })

        .catch(() => {
          this.alert.show('Error al obtener los beneficios.', 'warning');
        });
    },

    getCompanyTotalBenefitsElements() {
      URLBaseAPI.get(`/api/Company/getCompanyTotalBenefitsByCompanyId?CompanyId=${this.companyId}`)
        .then((response) => {
          this.maxCompanyBenefits = response.data;
        })

        .catch(() => {
          this.alert.show('Error al obtener el máximo de beneficios de la empresa.', 'warning');
        });
    },

    getAppliedElements() {
      URLBaseAPI.get(`/api/AppliedElement/getAppliedElements?employeeId=${this.user.userId}`)
        .then((response) => {
          this.appliedElements = response.data;
        })

        .catch(() => {
          this.alert.show('Error al obtener los beneficios aplicados.', 'warning');
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
      return this.filterAppliedElements.length;
    },

    getTotalActiveBenefits() {
      return this.filteredBenefits.filter((element) => element.status === 'Activo').length;
    },

    addAppliedElement(benefit) {
      if (this.maxCompanyBenefits - this.getTotalActiveAppliedBenefits() === 0) {
        this.alert.show('Se llegó al máximo de beneficios activos disponibles.', 'warning');
        return;
      }

      const alreadySelected = this.appliedElements.some(
        (applied) =>
          applied.elementName === benefit.elementName && applied.status === 'Activo'
      );

      if (alreadySelected) {
        this.alert.show('Este beneficio ya está seleccionado.', 'warning');
        return;
      }

      if (!this.user || !this.user.userId) {
        this.alert.show('El usuario actual no está definido.', 'warning');
        return;
      }

      if (!benefit.idElement) {
        this.alert.show('El beneficio no tiene un identificador válido.', 'warning');
        return;
      }

      if (benefit.calculationType === 'API') {
        if (benefit.calculationValue === 2) {
          // Seguro privado: dependientes
          this.pendingBenefit = benefit;
          this.extraFieldType = 'dependents';
          this.tempAmountDependents = null;
          this.tempPlanType = '';
          this.showExtraDataModal = true;
          return;
        }

        if (benefit.calculationValue === 3) {
          // Pensión voluntaria: plan A, B o C
          this.pendingBenefit = benefit;
          this.extraFieldType = 'plan';
          this.tempAmountDependents = null;
          this.tempPlanType = '';
          this.showExtraDataModal = true;
          return;
        }
      }

      // Si no requiere datos extra, se envía directo
      this.submitAppliedElement(benefit, null, null);
    },

    confirmExtraData() {
      if (!this.pendingBenefit) {
        this.cancelExtraData();
        return;
      }

      let amountDependents = null;
      let planType = null;

      if (this.extraFieldType === 'dependents') {
        amountDependents = this.tempAmountDependents;

        if (
          amountDependents == null ||
          !Number.isInteger(amountDependents) ||
          amountDependents <= 0
        ) {
          this.alert.show(
            'La cantidad de dependientes debe ser un número entero mayor que cero.',
            'warning'
          );
          return;
        }
      }

      if (this.extraFieldType === 'plan') {
        planType = (this.tempPlanType || '').toUpperCase();

        if (!['A', 'B', 'C'].includes(planType)) {
          this.alert.show('El tipo de plan debe ser A, B o C.', 'warning');
          return;
        }
      }

      this.submitAppliedElement(this.pendingBenefit, amountDependents, planType);
      this.cancelExtraData();
    },

    cancelExtraData() {
      this.showExtraDataModal = false;
      this.pendingBenefit = null;
      this.extraFieldType = null;
      this.tempAmountDependents = null;
      this.tempPlanType = '';
    },

    submitAppliedElement(benefit, amountDependents, planType) {
      URLBaseAPI.post('/api/AppliedElement/addAppliedElement', {
        UserId: this.user.userId,
        ElementId: benefit.idElement,
        ElementType: 'Beneficio',
        AmountDependents: amountDependents,
        PlanType: planType,
      })
        .then(() => {
          this.alert.show('Beneficio agregado exitosamente.', 'warning');
          this.getAppliedElements();
        })
        .catch(() => {
          this.alert.show('Error al agregar el beneficio.', 'warning');
        });
    },

    async deactivateAppliedElement(appliedElementId, status) {
      if (status === 'Inactivo') {
        this.alert.show('El elemento seleccionado ya está inactivo.', 'warning');
        return;
      }

      try {
        await URLBaseAPI.post('/api/AppliedElement/deactivateAppliedElement', {
          appliedElementId: appliedElementId,
        });

        this.alert.show('Beneficio desactivado correctamente.', 'warning');
        this.getAppliedElements();
      } catch {
        this.alert.show('Error al desactivar el beneficio.', 'warning');
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
