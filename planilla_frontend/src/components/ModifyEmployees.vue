<template>
  <!--Restrict page if user is Admin-->
  <div v-if="$session.user?.typeUser === 'Administrador'" class="d-flex flex-column text-center">
    <h3>Acceso restringido</h3>
    <p>Esta página no está disponible</p>
  </div>
  <div v-else class="d-flex flex-column">
  </div>
</template>

<script></script>

<style lang="scss" scoped></style>