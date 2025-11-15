<template>
  <div id="reportFilters" class="w-50">
    <div>
      <label for="selectPeriod" class="form-label fw-bold">Periodo</label>
      <select id="selectPeriod" class="form-select" v-model="selectedPayrollId">
        <option
          v-for="payroll in payrollList"
          :key="payroll.payrollId"
          :value="payroll.payrollId"
        >
          {{ payroll.periodLabel }}
        </option>
      </select>
    </div>
  </div>

  <div>
    <div id="buttons">
      <LinkButton text="Descargar PDF" />
    </div>
  </div>

  <div id="reportContent">
  
  </div>
</template>

<script>
import URLBaseAPI from '../../axiosAPIInstances.js';

export default {
  data() {
    return {
      payrollList: [],
      selectedPayrollId: null,
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
      }).then((response) => {
        this.payrollList = response.data || [];
          if (this.payrollList.length > 0) {
            this.selectedPayrollId = this.payrollList[0].payrollId;
          }
      })
        .catch((error) => {
          console.error('Error cargando las últimas planillas', error);
        });
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

</style>