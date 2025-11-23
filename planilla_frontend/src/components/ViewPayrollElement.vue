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
        @action="handleElementAction"
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
  import { useRouter } from 'vue-router';
  import { useGlobalAlert } from '@/utils/alerts.js';

  const router = useRouter();

  const alert = useGlobalAlert();
  
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
    { label: "Estado", key: "status"},
  ];

  var elements = ref([]);

  onMounted(() => {
    const userType = user.typeUser;
    GetPayrollElements(userType);
  });

  function GetPayrollElements(userType) {
    if (userType === "Administrador") GetCompanies();
    else {
      const companyId = user.companyUniqueId;
      GetCompanyPayrollElements(companyId, userType);
    }
  }

  async function GetCompanies() {
    isAdmin.value = true;
    await URLBaseAPI.get("/api/Company/GetAllCompaniesSummary")
        .then((response) => {
          companies.value = response.data;
        })
        .catch();
  }

  async function GetCompanyPayrollElements(idCompany, userType) {
    elements.value = [];
    await URLBaseAPI.get("/api/PayrollElement/GetPayRollElements", {params: {idCompany: idCompany}})
        .then((response) => {
          elements.value = response.data.map(e => ({
            ...e,
            paidBy: e.calculationType === 'API' ? 'Empleador' : e.paidBy,
            type: e.paidBy === 'Empleador' ? 'Beneficio' : 'Deducción',
            ...(userType === 'Empleador' && {
              action: `<button class="btn btn-sm btn-success me-1" data-id="${e.idElement}" data-action="edit">Editar</button>
                       <button class="btn btn-sm btn-danger" data-id="${e.idElement}" data-action="delete">Eliminar</button>`
            })
          }));

          if (userType === 'Empleador') {
            header.push({ label: "Acción", key: "action" });
          }
        })
        .catch((error) => {
          if (error.response) alert.show('Error: ' + error.response.data, 'warning'); 
          else alert.show('Error de red ' + error, 'warning');
        });
  }

  function editPayrollElement(PayrollElementId) {
    router.push({ name: 'EditarBeneficioODeduccion', params: { PEId: PayrollElementId}});
  }

  async function deletePayrollElement(PayrollElementId) {
    try {
      await URLBaseAPI.delete(`/api/PayrollElement/${PayrollElementId}`);
      alert.show('Elemento eliminado', 'success');
      elements.value = elements.value.filter(e => e.idElement != PayrollElementId);
    } catch (error) {
      alert.show('Error al eliminar elemento' + error, 'warning');
    }
  }

  function handleElementAction({ id, action }) {
  if (action === "edit") {
    editPayrollElement(id);
  } else if (action === "delete") {
    deletePayrollElement(id);
  }
}

</script>

<style lang="scss" scoped>
</style>
