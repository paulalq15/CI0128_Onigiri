<template>
  <div class="d-flex flex-column">
    <div class="container mt-5">
      <h1 class="display-4 text-center">Creación de planilla</h1>
      
      <div class="row justify-content-end">
        <div class="col-2"></div>
      </div>
      
      <div class="my-3">
        <button v-if="!payroll" type="button" class="btn btn-secondary btn-lg">Crear</button>
        <button v-else type="button" class="btn btn-secondary btn-lg">Pagar</button>
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
  </div>
</template>

<script>
import URLBaseAPI from '../axiosAPIInstances.js';

export default {
  name: "PayrollList",
  
  data() {
    return {
      payroll: null,
    };
  },
  
  methods: {
    getPayroll() {
      const companyId = this.$session.user?.companyId;
      URLBaseAPI.get(`/api/Payroll/summary?companyId=${companyId}`).then((response) => {
        this.payroll = response.data;
      });
    },
    
    formatDate(dateString) {
      if (!dateString)  return '';
      const date = new Date(dateString);
      return isNaN(date) ? '' : date.toLocaleDateString();
    }
  },
  mounted() {
    if (this.$session.user?.typeUser !== 'Empleador') return;
    this.getPayroll();
  },
};

</script>

<style lang="scss" scoped></style>