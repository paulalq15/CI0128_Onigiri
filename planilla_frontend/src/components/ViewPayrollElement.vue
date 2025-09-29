<template>
  <div class="d-flex flex-column" id="container">
    <!--Cuerpo-->
    <div class="d-flex justify-content-center align-items-center">
      <ReactiveObjectTable :title="title" :tableHeader="header" :tableElements="elements"/> 
    </div>
  </div>
</template>

<script setup>
  import ReactiveObjectTable from './ReactiveArrayTable.vue';

  const title = "Elementos de planilla"
  const header = [
    { label: "NOMBRE", key: "name" },
    { label: "APELLIDO", key: "surname" },
    { label: "TELEFONO", key: "tel" }
  ];
  var elements = [
    { name: "Angel", surname: "Mena", tel: "8456-3586"},
    { name: "Juan", surname: "Víquez", tel: "8456-3586" },    
  ]

  
</script>

<style lang="scss" scoped>
</style>
