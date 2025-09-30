<template>
  <div class="container p-5">
    <h3 class="display-5 text-center mb-3">{{ title }}</h3>

    <!-- Filtro empresa -->
    <div v-if="isAdmin" class="form-group mb-3">
      <label for="companyFilter">Empresa:</label>
      <select
        v-model="selectedCompanyLocal"
        class="form-select"
        id="companyFilter"
        @change="$emit('filter-company', selectedCompanyLocal)"
        style="width: 35%;">
        <option value="" disabled>Seleccione una empresa</option>
        <option
          v-for="company in companies"
          :key="company.companyUniqueId"
          :value="company.companyUniqueId">
          {{ company.companyName }}
        </option>
      </select>
    </div>

    <!-- Tabla -->
    <div class="table-responsive p-3"  style="background-color: white; border-radius: 10px;">
      <table class="table">
        <thead>
          <tr>
            <th scope="col">#</th>
            <th v-for="(head, index) in tableHeader" :key="index" scope="col">
              {{ head.label }}
            </th>
          </tr>
        </thead>
        <tbody>
          <template v-for="(element, index) in tableElements" :key="index">
            <tr>
              <td>{{ index + 1 }}</td>
              <td v-for="(head, j) in tableHeader" :key="j">
                {{ element[head.key] }}
              </td>
            </tr>
            <tr v-if="index < tableElements.length - 1" class="spacer">
              <td :colspan="tableHeader.length + 1"></td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
export default {
  name: "ReactiveObjectTable",
  props: {
    title: { type: String, default: "Tabla" },
    tableHeader: { type: Array, required: true },
    tableElements: { type: Array, required: true },
    companies: { type: Array, default: () => [] },
    isAdmin: { type: Boolean, default: false },
    selectedCompany: { type: String, default: ""}
  },

  data() {
    return {
      selectedCompanyLocal: this.selectedCompany
    };
  },
  
  watch: {
    selectedCompany(newVal) {
      this.selectedCompanyLocal = newVal;
    }
  }
};
</script>

<style scoped>
  table.table td {
    border: none;
  }
</style>
