<template>
    <div class="container mt-5">
        <h1 class="display-4 text-center">Lista de Empresas</h1>

        <div class="row justify-content-end">
            <div class="col-2"></div>
        </div>

    <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
            <tr>
                <th>Cédula Jurídica</th>
                <th>Nombre</th>
                <th>Teléfono</th>
                <th>Cantidad de Beneficios</th>
                <th>Frecuencia de Pago</th>
                <th>Día de Pago #1</th>
                <th>Día de Pago #2</th>
            </tr>
        </thead>

        <tbody>
            <tr v-for="(company, index) of companies" :key="index">
                <td>{{ company.companyId }}</td>
                <td>{{ company.companyName }}</td>
                <td>{{ company.telephone }}</td>
                <td>{{ company.maxBenefits }}</td>
                <td>{{ company.paymentFrequency }}</td>
                <td>{{ company.payDay1 }}</td>
                <td>{{ company.payDay2 }}</td>
            </tr>
        </tbody>
        </table>
    </div>
</template>

<script>
import axios from "axios";
    export default {
        name: "CompaniesList",

        data() {
            return {
                companies: [],
            };
        },

        methods: {
            getCompanies() {
                axios.get("https://localhost:7071/api/Company/getCompanies").then((response) => {
                this.companies = response.data;

                }).catch(error => {
                    console.error("Error al obtener compañías:", error);
                });
            },
        },

        created: function () {
            this.getCompanies();
        },
    };

</script>

<style lang="scss" scoped></style>
