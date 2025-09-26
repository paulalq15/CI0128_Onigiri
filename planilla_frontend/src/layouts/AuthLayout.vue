<template>
  <div class="d-flex flex-column min-vh-100" id="authLayout">
    <HeaderOnigiri />
    <main class="flex-fill">
      <router-view />
    </main>
    <FooterComp />
  </div>
</template>

<script>
import HeaderOnigiri from '../components/HeaderOnigiri.vue';
import FooterComp from '../components/FooterComp.vue';

export default {
  components: {
    HeaderOnigiri,
    FooterComp,
  },
};
</script>

<style lang="scss" scoped>
#authLayout {
  min-height: 100%;
  background: #596d53;
  background: linear-gradient(357deg, rgba(89, 109, 83, 1) 0%, rgba(225, 245, 219, 1) 80%);
}
</style>
