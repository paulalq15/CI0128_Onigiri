<template>
  <div class="d-flex flex-column" id="container">
    <!--Header Onigiri Solutions-->
    <HeaderOnigiri />
    <!--Cuerpo-->
    <div class="d-flex justify-content-center align-items-center">
      <!--Formulario-->
      <form class="form-signig p-5 h-100 d-inline-block" style="max-width: 550px; width: 100%;"  @submit.prevent="saveRegisterData">
        <h1 class="font-weigth-normal text-center mb-4">Registro</h1>
        <!--Primer nombre-->
        <InputType label="Primer nombre*" id="firstName" v-model="Persona.Nombre1"/>
        <span class="text-danger" v-if="errors.Nombre1">{{ errors.Nombre1 }}</span>
        <!--Segundo nombre-->
        <InputType label="Segundo nombre*" id="SecondName" v-model="Persona.Nombre2"/>
        <span class="text-danger" v-if="errors.Nombre2">{{ errors.Nombre2 }}</span>
        <!--Primer apellido-->
        <InputType label="Primer apellido*" id="firstSurname" v-model="Persona.Apellido1"/>
        <span class="text-danger" v-if="errors.Apellido1">{{ errors.Apellido1 }}</span>
        <!--Segundo apellido-->
        <InputType label="Segundo apellido*" id="secondSurname" v-model="Persona.Apellido2" />
        <span class="text-danger" v-if="errors.Apellido2">{{ errors.Apellido2 }}</span>
        <!--Cédula-->
        <InputType label="Cédula de indentidad*" id="IDCard" placeHolder="0-0000-0000" :max_length="11" v-model="Persona.Cedula" />
        <span class="text-danger" v-if="errors.Cedula">{{ errors.Cedula }}</span>
        <!--Número telefónico-->
        <InputType label="Número de teléfono*" id="cellphone" type="tel" placeHolder="0000-0000" :max_length="9" v-model="Persona.Telefono" />
        <span class="text-danger" v-if="errors.Telefono">{{ errors.Telefono }}</span>
        <!--Dirección: Provincia-->
        <div class="form-group mt-3">
          <label for="province">Provincia*</label>
          <select v-model="Direccion.selectedProvince" class="form-control" id="province" required @change="getCounties">
            <option value="" disabled selected>Seleccione una provincia</option>
            <option v-for="prov in provincias" :key="prov.value" :value="prov.value"> {{ prov.value }} </option>
          </select>
        </div>
        <span class="text-danger" v-if="errors.Provincia">{{ errors.Provincia }}</span>
        <!--Dirección: Cantón-->
        <div class="form-group mt-3">
          <label for="canton">Cantón*</label>
          <select v-model="Direccion.selectedCanton" class="form-control" id="canton" :disabled="!Direccion.selectedProvince" required @change="getDistricts">
            <option value="" disabled selected>Seleccione un cantón</option>
            <option v-for="canton in cantones" :key="canton.value" :value="canton.value"> {{ canton.value }} </option>
          </select>
        </div>
        <span class="text-danger" v-if="errors.Canton">{{ errors.Canton }}</span>
        <!--Dirección: Distrito-->
        <div class="form-group mt-3">
          <label for="distrit">Distrito*</label>
          <select v-model="Direccion.selectedDistrit" class="form-control" id="distrit" :disabled="!Direccion.selectedCanton" required @change="getZipCode">
            <option value="" disabled selected>Seleccione un distrito</option>
            <option v-for="distrit in distritos" :key="distrit.value" :value="distrit.value"> {{ distrit.value }} </option>
          </select>
        </div>
        <span class="text-danger" v-if="errors.Distrito">{{ errors.Distrito }}</span>
        <!--Dirección: Otras señas-->
        <div class="form-group mt-3">
          <label for="otherSigns">Otras señas</label>
          <textarea class="form-control" style="max-height: 200px;" id="otherSigns" rows="3" maxlength="200" v-model="Direccion.otherSigns" required></textarea>
        </div>
         <span class="text-danger" v-if="errors.otherSigns">{{ errors.otherSigns }}</span>
        <!--Fecha nacimiento-->
        <InputType label="Fecha de nacimiento*" id="birthday" type="date" v-model="Persona.FechaNacimiento" />
        <span class="text-danger" v-if="errors.FechaNacimiento">{{ errors.FechaNacimiento }}</span>
        <!--Correo electrónico-->
        <InputType label="Correo electrónico*" id="email" type="email" placeHolder="name@example.com" v-model="Persona.Correo" />
        <span class="text-danger" v-if="errors.Correo">{{ errors.Correo }}</span>
        <!--Contraseña-->
        <InputType label="Contraseña*" id="pwd" type="password" :min_length="8" :max_length="16" v-model="password" />
        <span class="text-danger" v-if="errors.Password">{{ errors.Password }}</span>
        <!--Confirmar contraseña-->
        <InputType label="Confirmar contraseña*" id="confPwd" type="password" :min_length="8" :max_length="16" v-model="confPassword" />
        <span class="text-danger" v-if="errors.ConfPassword">{{ errors.ConfPassword }}</span>
        <!--Términos y condiciones-->
        <p class="mt-4 mb-4" style="font-size: small;">Al continuar, aceptas nuestros <a href="#">Términos de servicio</a> y <a href="#">Política de privacidad</a></p>
        <!--Iniciar sesión-->
        <div class="d-flex justify-content-center mt-4 mb-3">
          <p class="me-3 fs-6">¿Ya tienes una cuenta?</p>
          <p><a href="#">Inicie Sesión</a></p>
        </div>
        <!--Botón confirmar-->
        <div class="form-group text-center">
          <LinkButton type="button" @click="saveRegisterData" text="Registrarse" />
        </div>
      </form>
    </div>
    <FooterComp />
  </div>
</template>

<script>
  import { toRaw } from "vue";
  
  // Enviar datos a la API
  import URLBaseAPI from '../axiosAPIInstances.js';

  // Componentes
  import HeaderOnigiri from './HeaderOnigiri.vue';
  import FooterComp from './FooterComp.vue';
  import LinkButton from './LinkButton.vue';
  import InputType from './InputType.vue';

  export default {
    components: {
      HeaderOnigiri,
      FooterComp,
      InputType,
      LinkButton
    },

    data() {
      return {
        // Guarda errores de validación
        errors: {},

        // Datos del formulario
        password: "",
        confPassword: "",
        zipCode: "",

        Persona: {
          Correo: "",
          Cedula: "",
          Nombre1: "",
          Nombre2:"",
          Apellido1: "",
          Apellido2: "",
          Telefono: "",
          FechaNacimiento: "",
          TipoPersona: "Empleador",
          Estado: "Inactivo"
        },

        // Datos para manejar la dirección
        Direccion: {
          selectedProvince: "",
          selectedCanton: "",
          selectedDistrit: "",
          otherSigns: "",
        },

        provincias: [],
        cantones: [],
        distritos: [],
      }
    },

    methods: {
      // Obtener lista de provincias
      GetProvince() {
        URLBaseAPI.get("/api/CountryDivision/Provinces")
        .then((response) => { this.provincias = response.data; });
      },

      // Obtener lista de cantones según la provincia seleccionada
      getCounties() {
        this.Direccion.selectedCanton = "";
        this.Direccion.selectedDistrit = "";
        this.cantones = [];
        this.distritos = [];

        URLBaseAPI.get("/api/CountryDivision/Counties", { params: {province: this.Direccion.selectedProvince}})
            .then((response) => { this.cantones = response.data; })
            .catch(console.error);
      },

      // Obtener lista de distritos según el cantón seleccionado
      getDistricts() {
        URLBaseAPI.get("/api/CountryDivision/Districts", { params: {province: this.Direccion.selectedProvince, county: this.Direccion.selectedCanton} })
            .then((response) => { this.distritos = response.data; })
            .catch(console.error);
      },

      // Obtener código postal según el distrito seleccionado
      getZipCode() {
        URLBaseAPI.get("/api/CountryDivision/ZipCode", { params: {province: this.Direccion.selectedProvince, county: this.Direccion.selectedCanton, district: this.Direccion.selectedDistrit} })
            .then((response) => { this.zipCode = response.data; })
            .catch(console.error);
      },

      // Validar los datos del formulario, agregar errores al objeto errors
      async validateForm() {
        this.errors = {};

        // Nombre y apellidos
        if (!this.Persona.Nombre1) this.errors.Nombre1 = "El primer nombre es obligatorio";
        if (!this.Persona.Nombre2) this.errors.Nombre2 = "El segundo nombre es obligatorio";
        if (!this.Persona.Apellido1) this.errors.Apellido1 = "El primer apellido es obligatorio";
        if (!this.Persona.Apellido2) this.errors.Apellido2 = "El segundo apellido es obligatorio";

        // Cédula
        if (!this.Persona.Cedula) this.errors.Cedula = "La cedula es obligatorio";
        else if (this.Persona.Cedula.length < 9) this.errors.Cedula = "La cedula debe tener al menos 9 caracteres";

        // Teléfono
        if (!this.Persona.Telefono) this.errors.Telefono = "El teléfono es obligatorio";
        else if (this.Persona.Telefono.length < 8) this.errors.Telefono = "El teléfono debe tener al menos 8 caracteres";

        // Fecha de nacimiento
        if (!this.Persona.FechaNacimiento) this.errors.FechaNacimiento = "La fecha de nacimiento es obligatoria";
        else {
          // Obtener fecha actual
          const today = new Date();
          // Convertir en objeto Date
          const birthDate = new Date(this.Persona.FechaNacimiento);
          // Calcular la edad obteniendo los años
          var age = today.getFullYear() - birthDate.getFullYear();

          if (age < 18) this.errors.FechaNacimiento = "Debe ser mayor de edad para registrarse";
        }

        // Dirección
        if (!this.Direccion.selectedProvince) this.errors.Provincia = "Debe seleccionar una provincia";
        if (!this.Direccion.selectedCanton) this.errors.Canton = "Debe seleccionar un cantón";
        if (!this.Direccion.selectedDistrit) this.errors.Distrito = "Debe seleccionar un distrito";
        if (!this.Direccion.otherSigns) this.errors.OtrasSeñas = "Debe ingresar otras señas";

        // Correo y contraseñas
        if (!this.Persona.Correo) this.errors.Correo = "El correo es obligatorio";
        else if (!/\S+@\S+\.\S+/.test(this.Persona.Correo)) this.errors.Correo = "Correo inválido";
        else {
          // Esperar la verificación de correo
          try {
            const response = await URLBaseAPI.get("/api/PersonaUsuario/emailCheck", { params: { email: this.Persona.Correo } });
            if (response.data > 0) {
              this.errors.Correo = "El correo ya está en uso";
            }
          } catch (error) {
            console.error(error);
          }
        }
        if (!this.password) this.errors.Password = "La contraseña es obligatoria";
        else if (this.password.length < 8) this.errors.Password = "La contraseña debe tener al menos 8 caracteres";
        else {
          // Validar una mayúscula
          if (!/[A-Z]/.test(this.password)) this.errors.Password = "La contraseña debe tener al menos una letra mayúscula";
          // Validar una minúscula
          if (!/[a-z]/.test(this.password)) this.errors.Password = "La contraseña debe tener al menos una letra minúscula";
          // Validar un número
          if (!/[0-9]/.test(this.password)) this.errors.Password = "La contraseña debe tener al menos un número";
          // Validar un carácter especial
          if (!/[!@#$%^&*(),.?":{}|<>]/.test(this.password)) this.errors.Password = "La contraseña debe tener al menos un carácter especial";
        }
        if (this.password !== this.confPassword) this.errors.ConfPassword = "Las contraseñas no coinciden";

        // Si no hay errores, el formulario es válido
        return Object.keys(this.errors).length === 0;
      },

      // Enviar datos del formulario para guardar
      async saveRegisterData() {
        if (!(await this.validateForm())) {
          // No continuar si hay errores
          return;
        }

        const registro = {
          personaData: toRaw(this.Persona),
          password: this.password,
          otherSigns: this.Direccion.otherSigns,
          zipCode: this.zipCode.value
        }

        console.log(registro);

        URLBaseAPI.post("/api/PersonaUsuario/register", registro)
            .then(response => { console.log("OK:", response.data); })
            .catch(error => {
                if (error.response) {
                  console.log("Error del backend:", error.response.data); // 👈 aquí está tu mensaje
                } else {
                  console.log("Error de red:", error.message);
                }
              }
            );
        }
    },

    mounted() {
      this.GetProvince();
    }
  }
</script>

<style lang="scss" scoped>
  #container {
    background: #596D53;
    background: linear-gradient(357deg, rgba(89, 109, 83, 1) 0%, rgba(225, 245, 219, 1) 80%);
  }

  .form-group {
    margin-bottom: 20px;
  }

  .text-danger {
    font-weight: bold;
    font-size: medium;
  }
</style>
