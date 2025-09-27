<template>
  <div class="d-flex flex-column min-vh-100" id="webpageLayout">
    <HeaderComp />
    <SidebarComp />
    <main class="flex-fill">
      <router-view />
    </main>
    <FooterComp />
  </div>
</template>

<script>
import HeaderComp from '../components/HeaderComp.vue';
import SidebarComp from '../components/SidebarComp.vue';
import FooterComp from '../components/FooterComp.vue';

export default {
  components: {
    HeaderComp,
    SidebarComp,
    FooterComp,
  },
};
</script>

<style lang="scss" scoped>
#webpageLayout {
  min-height: 100%;
  background: #596d53;
  background: linear-gradient(357deg, rgba(89, 109, 83, 1) 0%, rgba(225, 245, 219, 1) 80%);
}
</style>