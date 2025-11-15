<template>
  <div id="reportFilters" class="w-50">
    <div>
      <label for="selectPeriod" class="form-label fw-bold">Periodo</label>
      <select id="selectPeriod" class="form-select" v-model="selectedPayrollId">
        <option v-for="payroll in payrollList" :key="payroll.payrollId" :value="payroll.payrollId">
          {{ payroll.periodLabel }}
        </option>
      </select>
    </div>
  </div>

  <div id="reportContent" class="report-wrapper">
    <div v-if="isLoading" class="text-muted text-end">Cargando reporte</div>

    <div v-else-if="reportResult" class="report-card">
      <div class="report-header mb-4">
        <div class="row">
          <div class="col-md-6 mb-2">
            <div class="label">Nombre de la empresa</div>
            <div class="value">{{ reportResult.reportInfo.CompanyName }}</div>
          </div>
          <div class="col-md-6 mb-2">
            <div class="label">Nombre completo del empleado</div>
            <div class="value">{{ reportResult.reportInfo.EmployeeName }}</div>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6 mb-2">
            <div class="label">Tipo de contrato</div>
            <div class="value">{{ reportResult.reportInfo.EmployeeType }}</div>
          </div>
          <div class="col-md-6 mb-2">
            <div class="label">Fecha de pago</div>
            <div class="value">{{ formatDate(reportResult.reportInfo.PaymentDate) }}</div>
          </div>
        </div>
      </div>

      <div class="report-lines">
        <div v-for="(row, index) in reportResult.rows" :key="index" class="report-line" :class="lineClasses(row)">
          <div class="description">
            {{ row['Descripción'] }}
          </div>
          <div class="amount">
            {{ fmtCRC(row['Monto']) }}
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-muted">No hay datos para mostrar.</div>
  </div>
</template>

<script>
import URLBaseAPI from '../../axiosAPIInstances.js';

export default {
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
      const employeeId = Number(this.$session.user?.personId);

      URLBaseAPI.get('/api/Reports/employee/payroll-periods', {
        params: {
          companyId: companyId,
          employeeId: employeeId,
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
        reportCode: 'EmployeeDetailPayroll',
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
    fmtCRC(v) {
      return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: 'CRC',
        currencyDisplay: 'symbol',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }).format(Number(v ?? 0));
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
    lineClasses(row) {
      const desc = (row['Descripción'] || '').toString().toLowerCase();
      return {
        'line-gross': desc === 'salario bruto',
        'line-total': desc.includes('total'),
        'line-net': desc === 'pago neto',
      };
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
};
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
}

.report-line .description {
  flex: 1;
  text-align: left;
}

.report-line.line-gross, .report-line.line-total {
  font-weight: 600;
}

.report-line.line-net {
  font-weight: 700;
  font-size: 1.05rem;
  margin-top: 8px;
  padding-top: 8px;
}

.report-line .amount {
  min-width: 180px;
  text-align: right;
  font-weight: 500;
}
</style>