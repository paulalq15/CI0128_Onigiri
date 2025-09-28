<template>
    <div class="container mt-5">
        <h1 class="display-4 text-center">Lista de Empleados</h1>

        <div class="row justify-content-end">
            <div class="col-2"></div>
        </div>

    <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
        <thead>
            <tr>
                <th>Cédula</th>
                <th>Primer Nombre</th>
                <th>Segundo Nombre</th>
                <th>Primer Apellido</th>
                <th>Segundo Apellido</th>
                <th>Fecha Nacimiento</th>
                <th>Email</th>
                <th>Tipo Contrato</th>
                <th>Puesto</th>
                <th>Departamento</th>
            </tr>
        </thead>

        <tbody>
            <tr v-for="(employee, index) of employees" :key="index">
                <td>{{ employee.cedula }}</td>
                <td>{{ employee.name1 }}</td>
                <td>{{ employee.name2 }}</td>
                <td>{{ employee.surname1 }}</td>
                <td>{{ employee.surname2 }}</td>
                <td>{{ }}</td>
                <td>{{ }}</td>
                <td>{{ }}</td>
                <td>{{ }}</td>
                <td>{{ }}</td>
            </tr>
        </tbody>
        </table>
    </div>
</template>

<script>
import axios from "axios";
    export default {
        name: "EmployeesList",

        data() {
            return {
                employees: [],
            };
        },

        methods: {
            getEmployees() {
                axios.get("https://localhost:7115/api/PersonUser/getEmployees").then((response) => {
                    this.employees = response.data;
                });
            },
        },

        created: function () {
            this.getEmployees();
        },
    };

</script>

<style lang="scss" scoped></style>
