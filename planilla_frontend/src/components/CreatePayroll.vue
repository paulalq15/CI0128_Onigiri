<template>
  <div class="d-flex flex-column">
    <div class="container mt-5">
      <h1 class="display-4 text-center">Creación de planilla</h1>

      <div v-if="!payroll" class="row align-items-end my-4">
        <div class="col-auto d-flex align-items-end gap-4">
          <div>
            <label class="form-label mb-1">Mes a pagar</label>
            <input type="month" class="form-control" v-model="selectedMonth" />
          </div>

          <div>
            <label class="form-label mb-1 d-block">Periodo</label>
            <div class="btn-group" role="group">
              <button
                type="button"
                class="btn"
                :class="half === '01' ? 'btn-secondary' : 'btn-outline-secondary'"
                @click="half = '01'"
              >
                1 al 15
              </button>
              <button
                type="button"
                class="btn"
                :class="half === '16' ? 'btn-secondary' : 'btn-outline-secondary'"
                @click="half = '16'"
              >
                16 al 30
              </button>
            </div>
          </div>
        </div>

        <div class="col ms-auto text-end">
          <button
            type="button"
            class="btn btn-secondary btn-lg"
            :disabled="!selectedMonth"
            @click="createPayroll"
          >
            Crear
          </button>
        </div>
      </div>

      <!-- Payroll exists and needs to be paid -->
      <div v-else class="row align-items-center justify-content-between my-4">
        <div class="col-auto">
          <div class="fs-5 mb-1">Fecha inicial: {{ formatDate(payroll.dateFrom) }}</div>
          <div class="fs-5">Fecha final: {{ formatDate(payroll.dateTo) }}</div>
        </div>
        <div class="col-auto text-end">
          <button type="button" class="btn btn-secondary btn-lg" @click="payPayroll">Pagar</button>
        </div>
      </div>

      <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
          <tr>
            <th>Fecha de pago</th>
            <th>Total salarios brutos</th>
            <th>Total deducciones empleador</th>
            <th>Total deducciones empleado</th>
            <th>Total beneficios</th>
            <th>Total salarios netos</th>
            <th>Total costo planilla</th>
          </tr>
        </thead>

        <tbody>
          <tr v-if="payroll">
            <td>{{ formatDate(payroll.payDate) }}</td>
            <td>{{ payroll.totalGrossSalaries }}</td>
            <td>{{ payroll.totalEmployerDeductions }}</td>
            <td>{{ payroll.totalEmployeeDeductions }}</td>
            <td>{{ payroll.totalBenefits }}</td>
            <td>{{ payroll.totalNetEmployee }}</td>
            <td>{{ payroll.totalEmployerCost }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!--Toast messages-->
    <div class="toast-container position-fixed top-0 end-0 p-3">
      <div
        v-if="showToast"
        class="toast show align-items-center text-white border-0"
        :class="toastType"
        role="alert"
        aria-live="assertive"
        aria-atomic="true"
      >
        <div class="d-flex">
          <div class="toast-body">
            {{ toastMessage }}
          </div>
          <button
            type="button"
            class="btn-close btn-close-white me-2 m-auto"
            @click="showToast = false"
          ></button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import URLBaseAPI from '../axiosAPIInstances.js';

export default {
  name: 'PayrollList',

  data() {
    return {
      payroll: null,
      selectedMonth: '',
      half: '01',
      dateFrom: null,
      dateTo: null,
      showToast: false,
      toastMessage: '',
      toastType: 'bg-success',
      toastTimeout: 3000,
    };
  },
  methods: {
    getDateFrom() {
      if (!this.selectedMonth) return '';
      return `${this.selectedMonth}-${this.half}`;
    },
    getDateTo() {
      if (!this.selectedMonth) return '';
      const [y, m] = this.selectedMonth.split('-').map(Number);
      if (this.half === '01') return `${this.selectedMonth}-15`;
      const lastDay = new Date(y, m, 0).getDate();
      return `${this.selectedMonth}-${String(lastDay).padStart(2, '0')}`;
    },
    createPayroll() {
      this.dateFrom = this.getDateFrom();
      this.dateTo = this.getDateTo();
      var self = this;

      const params = {
        companyId: this.$session.user?.companyUniqueId,
        personId: Number(this.$session.user?.personId),
        dateFrom: this.dateFrom,
        dateTo: this.dateTo,
      };

      URLBaseAPI.post('/api/Payroll', null, { params })
        .then((response) => {
          this.payroll = response.data;

          self.toastMessage = 'Planilla creada correctamente';
          self.toastType = 'bg-success';
          self.showToast = true;
          setTimeout(function () {
            self.showToast = false;
          }, self.toastTimeout);
        })
        .catch(function (error) {
          var msg =
            error && error.response && error.response.data
              ? error.response.data
              : 'Error inesperado al crear la planilla';
          self.toastMessage = msg;
          self.toastType = 'bg-danger';
          self.showToast = true;

          setTimeout(function () {
            self.showToast = false;
          }, self.toastTimeout);
        });
    },
    payPayroll() {
      this.dateFrom = this.getDateFrom();
      this.dateTo = this.getDateTo();
      var self = this;

      const params = {
        payrollId: this.payroll.companyPayrollId,
        personId: Number(this.$session.user?.personId),
      };

      URLBaseAPI.post('/api/Payroll/payment', null, { params })
        .then((response) => {
          this.payroll = response.data;

          self.toastMessage = 'Planilla pagada correctamente';
          self.toastType = 'bg-success';
          self.showToast = true;
          setTimeout(function () {
            self.showToast = false;
          }, self.toastTimeout);
        })
        .catch(function (error) {
          var msg =
            error && error.response && error.response.data
              ? error.response.data
              : 'Error inesperado al pagar la planilla';
          self.toastMessage = msg;
          self.toastType = 'bg-danger';
          self.showToast = true;

          setTimeout(function () {
            self.showToast = false;
          }, self.toastTimeout);
        });
    },
    getPayroll() {
      const companyId = this.$session.user?.companyUniqueId;
      URLBaseAPI.get(`/api/Payroll/summary?companyId=${companyId}`).then((response) => {
        this.payroll = response.data;
        this.dateFrom = response.data?.dateFrom || null;
        this.dateTo = response.data?.dateTo || null;
      });
    },

    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return isNaN(date)
        ? ''
        : new Intl.DateTimeFormat('es-CR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
          }).format(date);
    },
  },
  mounted() {
    if (this.$session.user?.typeUser !== 'Empleador') return;
    this.getPayroll();
  },
};
</script>

<style lang="scss" scoped></style>
