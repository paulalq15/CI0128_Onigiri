<template>
  <div class="container p-4">
    <div class="mb-4 pb-4 px-5">
      <h4 class="mb-1">¡Hola, Paula Lopez!</h4>
      <p class="text-muted mb-0">Selecciona lo que quieres hacer hoy</p>
    </div>

    <div class="row g-5 px-5">
      <div class="col-12 col-sm-6 col-md-4 col-lg-3 col-xl-2">
        <home-page-card title="Empresas" icon="bi bi-buildings-fill" to="/CrearEmpresa" />
      </div>
      <div class="col-12 col-sm-6 col-md-4 col-lg-3 col-xl-2">
        <home-page-card title="Empleados" icon="bi bi-people-fill" to="/" />
      </div>
      <div class="col-12 col-sm-6 col-md-4 col-lg-3 col-xl-2">
        <home-page-card title="Timesheets" icon="bi bi-calendar-week-fill" to="/" />
      </div>
      <div class="col-12 col-sm-6 col-md-4 col-lg-3 col-xl-2">
        <home-page-card title="Planilla" icon="bi bi-calculator-fill" to="/" />
      </div>
      <div class="col-12 col-sm-6 col-md-4 col-lg-3 col-xl-2">
        <home-page-card title="Pagos" icon="bi bi-credit-card-fill" to="/" />
      </div>
      <div class="col-12 col-sm-6 col-md-4 col-lg-3 col-xl-2">
        <home-page-card title="Reportes" icon="bi bi-bar-chart-fill" to="/" />
      </div>
    </div>
  </div>
</template>

<script>
import HomePageCard from './HomePageCard.vue';
export default {
  components: {
    HomePageCard,
  },
};
</script>

<style lang="scss" scoped></style>