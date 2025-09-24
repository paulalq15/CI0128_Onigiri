<template>
  <router-view></router-view>
  <!--Alerta global-->
    <div v-if="alert.message"
         :class="['alert', `alert-${alert.type}`, 'alert-dismissible', 'fade', 'show']"
         role="alert"
         style="position:fixed; top:20px; right:20px; min-width:250px;">
      {{ alert.message }}
      <button type="button" class="btn-close" @click="alert.clear()"></button>
    </div>
</template>

<script setup>
  // Código que permite mantener una alerta entre pantallas
  import { useGlobalAlert } from '@/utils/alerts'

  const alert = useGlobalAlert()
</script>

<style>
  body {
      background: #596D53;
      background: linear-gradient(357deg, rgba(89, 109, 83, 1) 0%, rgba(225, 245, 219, 1) 80%);
    }
</style>