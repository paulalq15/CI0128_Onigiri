<template>
  <div class="d-flex flex-column align-items-center text-center p-4">
    <i class="bi bi-shield-lock" style="font-size: 3rem"></i>
    <h3 class="mt-3 mb-2">Acceso restringido</h3>
    <p class="text-muted mb-4">No tiene permisos para ver esta página.</p>
    <router-link class="btn btn-outline-secondary" :to="{ name: 'Home Page' }">
      Volver al inicio
    </router-link>
  </div>
</template>

<script></script>

<style lang="scss" scoped></style>
