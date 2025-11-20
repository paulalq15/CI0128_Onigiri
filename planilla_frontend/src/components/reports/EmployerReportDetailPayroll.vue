<template>
  <div id="reportFilters">
    <div>
      <label for="selectPeriod" class="form-label fw-bold">Periodo</label>
      <select id="selectPeriod" class="form-select" v-model="selectedPayrollId">
        <option v-for="payroll in payrollList" :key="payroll.payrollId" :value="payroll.payrollId">
          {{ payroll.periodLabel }}
        </option>
      </select>
    </div>
  </div>
</template>


<script>
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import URLBaseAPI from '../../axiosAPIInstances.js';
import LinkButton from '../LinkButton.vue';

export default {
  components: {
    LinkButton,
  },

  data() {
    return {
      payrollList: [],
      selectedPayrollId: null,
      reportResult: null,
      isLoading: false,
      showToast: false,
      toastMessage: '',
      toastTimeout: 2000,
    };
  },

  methods: {
    getPayrollList() {
      const companyId = this.$session.user?.companyUniqueId;

      URLBaseAPI.get('/api/Reports/employer/payroll-periods', {
        params: {
          companyId: companyId,
        },
      })

        .then((response) => {
          this.payrollList = response.data || [];
          if (this.payrollList.length > 0) {
            this.selectedPayrollId = this.payrollList[0].payrollId;
          }
        })

        .catch((error) => {
          const data = error && error.response && error.response.data ? error.response.data : null;
          const msg =
            typeof data === 'string'
              ? data
              : (data && (data.message || data.detail)) || 'Error cargando las últimas planillas';

          this.toastMessage = msg;
          this.toastType = 'bg-danger';
          this.showToast = true;

          setTimeout(function () {
            this.showToast = false;

          }, this.toastTimeout);
        });
    },

    loadReport() {
      if (!this.selectedPayrollId) return;

      const companyId = this.$session.user?.companyUniqueId;
      const employeeId = Number(this.$session.user?.personId);

      const params = {
        reportCode: 'EmployerDetailPayroll',
        companyId,
        employeeId,
        payrollId: this.selectedPayrollId,
      };

      this.isLoading = true;
      URLBaseAPI.post('/api/Reports/data', params)
        .then((response) => {
          this.reportResult = response.data;
        })

        .catch((error) => {
          const data = error && error.response && error.response.data ? error.response.data : null;
          const msg =
            typeof data === 'string'
              ? data
              : (data && (data.message || data.detail)) || 'Error cargando el detalle de planilla';

          this.toastMessage = msg;
          this.toastType = 'bg-danger';
          this.showToast = true;

          setTimeout(function () {
            this.showToast = false;
          }, this.toastTimeout);

          this.reportResult = null;
        })

        .finally(() => {
          this.isLoading = false;
        });
    },
  },

  watch: {
    selectedPayrollId() {
      this.loadReport();
    },
  },

  mounted() {
    this.getPayrollList();
  },
}

</script>


<style lang="scss" scoped>

#reportFilters {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 12px;
  margin-bottom: 20px;
}

#reportFilters > div {
  flex: 1;
  text-align: left;
}

#buttons {
  margin-top: 4px;
}

#buttons :deep(button),
#buttons button {
  width: auto;
  padding: 6px 18px;
  white-space: nowrap;
}

.report-wrapper {
  max-width: 800px;
  margin: 0 auto 40px;
}

.report-card {
  background: rgba(255, 255, 255, 0.9);
  border-radius: 12px;
  padding: 24px 32px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
}

.report-header .label {
  font-size: 0.85rem;
  font-weight: 600;
  color: #666;
}

.report-header .value {
  font-size: 1rem;
}

.report-lines {
  margin-top: 8px;
}

.report-line {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  padding: 4px 0;
  border-bottom: 1px solid rgba(0, 0, 0, 0.04);
  font-weight: 400;
}

.report-line .description {
  flex: 1;
  text-align: left;
}

.report-line .amount {
  min-width: 180px;
  text-align: right;
  font-weight: 400;
}

.report-line.line-gross .description,
.report-line.line-gross .amount,
.report-line.line-total .description,
.report-line.line-total .amount {
  font-weight: 600;
}

.report-line.line-net .description,
.report-line.line-net .amount {
  font-weight: 700;
  font-size: 1.05rem;
  margin-top: 8px;
  padding-top: 8px;
}

</style>
