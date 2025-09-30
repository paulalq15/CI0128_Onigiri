<template>
  <div class="d-flex flex-column" id="container">
    <!--Cuerpo-->
    <div class="d-flex flex-column justify-content-center align-items-center">
      <ReactiveObjectTable
        :title="title"
        :tableHeader="header"
        :tableElements="elements"
        :companies="companies"
        :isAdmin="isAdmin"
        :selectedCompany="selectedCompany"
        @filter-company="GetCompanyPayrollElements"
      /> 
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
  var isAdmin = ref(false);
  var companies = ref([]);
  var selectedCompany = ref("");

  const title = "Elementos de planilla"
  const header = [
    { label: "Elemento", key: "elementName" },
    { label: "Monto/Porcentaje", key: "calculationType" },
    { label: "Valor", key: "calculationValue" },
    { label: "Tipo", key: "type"},
    { label: "Pagado por", key: "paidBy" },
    { label: "Estado", key: "status"}
  ];
  var elements = ref([]);

  onMounted(() => {
    GetPayrollElements();
  });

  function GetPayrollElements() {
    const userType = user.typeUser;
    console.log("Tipo de usuario: " + userType);
    if (userType === "Administrador") GetCompanies();
    else {
      const companyId = user.companyUniqueId;
      GetCompanyPayrollElements(companyId);
    }
  }

  async function GetCompanies() {
    isAdmin.value = true;
    await URLBaseAPI.get("/api/Company/GetAllCompaniesSummary")
        .then((response) => {
          companies.value = response.data;
          console.log("Empresas: ", companies.value);
        })
        .catch();
  }

  async function GetCompanyPayrollElements(idCompany) {
    console.log("Company id: " + idCompany);
    await URLBaseAPI.get("/api/PayrollElement/GetPayRollElements", {params: {idCompany: idCompany}})
        .then((response) => {
          console.log(response.data);
          elements.value = response.data.map(e => ({
            ...e,
            type: e.paidBy === 'Empleador' ? 'Beneficio' : 'Deducción'
          }));
          elements.value.paidBy == elements.value.calculationType === 'API' ? 'Empleador' : elements.value.paidBy;
        })
        .catch((error) => {
          if (error.response) console.log('Error del backend:', error.response.data);
          else console.log('Error de red:', error.message);
        });
  }
</script>

<style lang="scss" scoped>
</style>
