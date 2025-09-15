<template>
    <body class="d-flex flex-column min-vh-100">
        <HeaderComp />

        <h1 class="text-center">Crear nueva empresa</h1>

        <div class="container py-4 flex-fill d-flex justify-content-center">   
            <form action="/" method="post" class="row g-3 needs-validation" novalidate style="width: 800px;">
                <div class="mb-3 col-xl-6 col-sm-12">
                    <label for="companyId" class="form-label">Cédula Jurídica</label>
                    <input type="text" class="form-control" id="companyId" placeholder="3-000-000000" required>
                    <div class="invalid-feedback">
                        Ingrese la cédula jurídica 
                    </div>
                </div>
                <div class="mb-3 col-xl-6 col-sm-12">
                    <label for="companyName" class="form-label">Nombre Empresa</label>
                    <input type="text" class="form-control" id="companyName" required>
                    <div class="invalid-feedback">
                        Ingrese el nombre de la empresa
                    </div>
                </div>
                <div class="mb-3 col-xl-6 col-sm-12">
                    <label for="Province" class="form-label">Provincia</label>
                    <select class="form-select" id="Province" required>
                        <option selected disabled value="">Seleccione una Provincia</option>
                        <option>...</option>
                    </select>
                    <div class="invalid-feedback">
                        Seleccione una provincia
                    </div>
                </div>
                <div class="mb-3 col-xl-6 col-sm-12">
                    <label for="County" class="form-label">Cantón</label>
                    <select class="form-select" id="County" required>
                        <option selected disabled value="">Seleccione un cantón</option>
                        <option>...</option>
                    </select>
                    <div class="invalid-feedback">
                        Seleccione un cantón
                    </div>
                </div>
                <div class="mb-3 col-xl-6 col-sm-12">
                    <label for="District" class="form-label">Distrito</label>
                    <select class="form-select" id="District" required>
                        <option selected disabled value="">Seleccione un distrito</option>
                        <option>...</option>
                    </select>
                    <div class="invalid-feedback">
                        Seleccione un distrito
                    </div>
                </div>
                <div class="mb-3 col-xl-6 col-sm-12">
                    <label for="ZipCode" class="form-label">Código Postal</label>
                    <input type="text" class="form-control" id="ZipCode" readonly>
                </div>
                <div class="mb-3">
                    <label for="AddressDetails" class="form-label">Otras señas</label>
                    <textarea class="form-control" id="AddressDetails" rows="3" placeholder="Otras señas" required></textarea>
                    <div class="invalid-feedback">
                        Ingrese las señas
                    </div>
                </div>
                <div class="mb-3">
                    <label for="Telephone" class="form-label">Teléfono</label>
                    <input type="text" class="form-control" id="Telephone" placeholder="2222-2222">
                    <div class="invalid-feedback">
                        Ingrese el número de teléfono
                    </div>
                </div>
                <div class="mb-3">
                    <label for="MaxBenefits" class="form-label">Cantidad máxima de beneficios</label>
                    <input type="number" class="form-control" id="MaxBenefits" required>
                    <div class="invalid-feedback">
                        Ingrese la cantidad máxima de beneficios que puede seleccionar un empleado
                    </div>
                </div>
                <div class="mb-3">
                    <label for="PaymentFrequency" class="form-label">Frecuencia de pago</label>
                    <select class="form-select" id="PaymentFrequency" required>
                        <option selected disabled value="">Selecciona la frecuencia de pago de la planilla</option>
                        <option>Mensual</option>
                        <option>Quincenal</option>
                    </select>
                    <div class="invalid-feedback">
                        Seleccione la frecuencia de pago
                    </div>
                </div>
                <div class="mb-3 col-xl-6 col-sm-12">
                    <label for="PayDay1" class="form-label">Primer día de pago</label>
                    <select class="form-select" id="PayDay1" required>
                        <option selected disabled value="">Día de pago</option>
                        <option>...</option>
                    </select>
                    <div class="invalid-feedback">
                        Please select a valid state.
                    </div>
                </div>
                <div class="mb-3 col-xl-6 col-sm-12">
                    <label for="PayDay2" class="form-label">Segundo día de pago</label>
                    <select class="form-select" id="PayDay2" required>
                        <option selected disabled value="">Día de pago</option>
                        <option>...</option>
                    </select>
                    <div class="invalid-feedback">
                        Seleccione el segundo día de pago
                    </div>
                </div>
                <div class="col-md-4">
                    <button class="btn btn-primary" type="submit">Submit form</button>
                </div>
            </form>
        </div>
        <FooterComp />
    </body>
</template>

<script>
    import HeaderComp from './HeaderComp.vue';
    import FooterComp from './FooterComp.vue';
    export default {
        components: {
        HeaderComp,
        FooterComp,
        },
        methods: {
            initBootstrapValidation() {
                const forms = this.$el.querySelectorAll('.needs-validation')

                Array.from(forms).forEach((form) => {
                    form.addEventListener('submit', (event) => {
                        if (!form.checkValidity()) {
                            event.preventDefault()
                            event.stopPropagation()
                        } else {
                            event.preventDefault()
                        }
                        form.classList.add('was-validated')
                    }, false)
                })
            },
        },
        mounted() {
            this.initBootstrapValidation()
        },
    };
</script>

<style lang="scss" scoped>
    body {
        background: #596D53;
        background: linear-gradient(357deg, rgba(89, 109, 83, 1) 0%, rgba(225, 245, 219, 1) 80%);
    }
</style>