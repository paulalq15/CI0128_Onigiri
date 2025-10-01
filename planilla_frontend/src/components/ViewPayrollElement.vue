<template>
  <div class="d-flex justify-content-center p-5 ms-5 me-5">
    <div class="w-100 ps-5 pe-5 ms-4 me-4">
      <h3 class="display-5 text-center mb-4">Elementos de planilla</h3>
      <!--Filtros-->
      <div class="d-flex mb-4 gap-3">
        <!-- Filtro empresa (solo admin) -->
        <div v-if="isAdmin" class="form-group">
          <label for="companyFilter">Empresa:</label>
          <select
            v-model="selectedCompany"
            class="form-select"
            id="companyFilter"
            @change="GetCompanyPayrollElements(selectedCompany)">
            <option value="" disabled>Seleccione una empresa</option>
            <option
              v-for="company in companies"
              :key="company.companyUniqueId"
              :value="company.companyUniqueId">
              {{ company.companyName }}
            </option>
          </select>
        </div>

        <!-- Filtro tipo deducción o beneficio -->
        <div class="form-group">
          <label for="typeElementFilter">Tipo de elemento:</label>
          <select
            v-model="selectedElementType"
            class="form-select"
            id="typeElementFilter">
            <option value="" disabled>Seleccione un elemento</option>
            <option value="Todos">Todos</option>
            <option value="Beneficio">Beneficio</option>
            <option value="Deducción">Deducción</option>
          </select>
        </div>
      </div>
      <!--tabla-->
      <ReactiveObjectTable
        :tableHeader="header"
        :tableElements="filteredElements"
      />     
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from 'vue';

  // Base URL de la API
  import URLBaseAPI from '../axiosAPIInstances.js';
  import { useSession } from '@/utils/useSession';

  import ReactiveObjectTable from './ReactiveArrayTable.vue';

  const  {user} = useSession();
  var isAdmin = ref(false);
  var companies = ref([]);
  var selectedCompany = ref("");
  var selectedElementType = ref("Todos");

  const filteredElements = computed(() => {
    if (selectedElementType.value === "Todos") return elements.value;
    return elements.value.filter(el => el.type === selectedElementType.value);
  })

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
    elements.value = [];
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
