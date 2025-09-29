<template>
  <div class="d-flex flex-column" id="container">
    <!--Cuerpo-->
    <div class="d-flex justify-content-center align-items-center">
      <ReactiveObjectTable :title="title" :tableHeader="header" :tableElements="elements"/> 
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';

  // Base URL de la API
  import URLBaseAPI from '../axiosAPIInstances.js';
  import { useSession } from '@/utils/useSession';

  import ReactiveObjectTable from './ReactiveArrayTable.vue';

  const  {user} = useSession();

  const title = "Elementos de planilla"
  const header = [
    { label: "ELEMENTO", key: "elementName" },
    { label: "TIPO", key: "calculationType" },
    { label: "VALOR", key: "calculationValue" },
    { label: "PAGADO POR", key: "paidBy" },
    { label: "ESTADO", key: "status"}
  ];
  var elements = ref([]);

  onMounted(() => {
    GetPayrollElements();
  });

  async function GetPayrollElements() {
    const companyId = user.companyUniqueId;
    console.log("Company id: " + companyId)
    await URLBaseAPI.get("/api/PayrollElement/GetPayRollElements", {params: {idCompany: companyId}})
        .then((response) => {
          console.log(response.data);
          elements.value = response.data; // sin mapear, todo queda tal cual
        })
        .catch((error) => {
          if (error.response) console.log('Error del backend:', error.response.data);
          else console.log('Error de red:', error.message);
        });
  }
</script>

<style lang="scss" scoped>
</style>
