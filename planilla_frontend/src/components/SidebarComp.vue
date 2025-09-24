<template>
  <div class="offcanvas offcanvas-end" tabindex="-1" id="appSidebar" aria-labelledby="sidebarLabel" data-bs-backdrop="false" data-bs-scroll="true">
    <div class="offcanvas-header">
      <h5 id="sidebarLabel">Menú</h5>
      <button type="button" class="btn-close shadow-none" data-bs-dismiss="offcanvas" aria-label="Cerrar"></button>
    </div>
    <div class="offcanvas-body">
      <ul class="list-group">
        <RouterLink class="list-group-item list-group-item-action" to="/" @click="closeSidebar">Página principal</RouterLink>
      </ul>
    </div>
  </div>
</template>

<script>
</script>

<style lang="scss" scoped>
</style>