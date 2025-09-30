<template>
  <!--Restrict page if user is Admin-->
  <div v-if="$session.user?.typeUser === 'Administrador'" class="d-flex flex-column text-center">
    <h3>Acceso restringido</h3>
    <p>Esta página no está disponible</p>
  </div>
  <div v-else class="d-flex flex-column"></div>

<!--Page Content:-->
<div class="container mt-5">
        <h1 class="display-4 text-center">Lista de Beneficios</h1>

        <div class="row justify-content-end">
            <div class="col-2"></div>
        </div>

    <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
            <tr>
                <th>Nombre</th>
            </tr>
        </thead>

        <tbody>
            <!-- Benefits row(s) -->
            <tr v-for="(benefit, index) of benefits" :key="index">
                <td>{{ benefit.elementName }}</td>
            </tr>

            <!-- Counter row -->
            <tr>
              <td style="text-align: center; width: 50px; height: 50px; border: 1px solid #000; font-weight: bold; vertical-align: middle;">
                Total Benefits: {{ benefits.length }}
              </td>
            </tr>
        </tbody>
    </table>
</div>

</template>

<script>
  import axios from "axios";

  export default {
    name: "BenefitsList",

    data() {
      return {
        benefits: [],
      };
    },

    methods: {
      getBenefits() {
        axios.get(`https://localhost:7071/api/PayrollElement/getPayrollElements?paidBy=Empleador`)
        .then((response) => {
        this.benefits = response.data;

        }).catch(error => {
          console.error("Error al obtener los beneficios:", error);
        });
      },
    },

    created: function () {
      this.getBenefits();
    },
  };

</script>

<style lang="scss" scoped></style>
