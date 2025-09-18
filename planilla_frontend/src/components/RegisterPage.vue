<template>
  <div class="d-flex flex-column" id="container">
      <!--Header Onigiri Solutions-->
      <HeaderOnigiri />
      <!--Cuerpo-->
      <div class="d-flex justify-content-center align-items-center">
        <!--Formulario-->
        <form class="form-signig p-5 h-100 d-inline-block" style="max-width: 550px; width: 100%;">
          <h1 class="font-weigth-normal text-center mb-4">Registro</h1>
          <!--Primer nombre-->
          <InputType label="Primer nombre*" id="firstName" v-model="firstName"/>
          <!--Segundo nombre-->
          <InputType label="Segundo nombre*" id="SecondName" v-model="secondName"/>
          <!--Primer apellido-->
          <InputType label="Primer apellido*" id="firstSurname" v-model="firstSurname"/>
          <!--Segundo apellido-->
          <InputType label="Segundo apellido*" id="secondSurname" v-model="secondSurname" />
          <!--Cédula-->
          <InputType label="Cédula de indentidad*" id="IDCard" placeHolder="0-0000-0000" v-model="idCard" />
          <!--Número telefónico-->
          <InputType label="Número de teléfono*" id="cellphone" type="tel" placeHolder="0000-0000" v-model="number" />
          <!--Dirección: Provincia-->
          <div class="form-group">
            <label for="province">Provincia*</label>
            <select v-model="selectedProvince" class="form-control" id="province" required>
              <option value="" disabled selected>Seleccione una provincia</option>
              <option v-for="(cantones, prov) in cantonesPorProvinciaCR" :key="prov" :value="prov"> {{ prov }} </option>
            </select>
          </div>
          <!--Dirección: Cantón-->
          <div class="form-group">
            <label for="canton">Cantón*</label>
            <select v-model="selectedCanton" class="form-control" id="canton" :disabled="!selectedProvince" required>
              <option value="" disabled selected>Seleccione un cantón</option>
              <option v-for="(distritos, canton) in selectProvince()" :key="canton" :value="canton"> {{ canton }} </option>
            </select>
          </div>
          <!--Dirección: Distrito-->
          <div class="form-group">
            <label for="distrit">Distrito*</label>
            <select v-model="selectedDistrit" class="form-control" id="distrit" :disabled="!selectedCanton" required>
              <option value="" disabled selected>Seleccione un distrito</option>
              <option v-for="distrit in selectCanton()" :key="distrit" :value="distrit"> {{ distrit }} </option>
            </select>
          </div>
          <!--Dirección: Otras señas-->
          <div class="form-group">
            <label for="otherSigns">Otras señas</label>
            <textarea class="form-control" style="max-height: 200px;" id="otherSigns" rows="3" v-model="otherSigns"></textarea>
          </div>
          <!--Fecha nacimiento-->
          <InputType label="Fecha de nacimiento*" id="birthday" type="date" v-model="birthday" />
          <!--Correo electrónico-->
          <InputType label="Correo electrónico*" id="email" type="email" placeHolder="name@example.com" v-model="email" />
          <!--Contraseña-->
          <InputType label="Contraseña*" id="pwd" type="password" v-model="password" />
          <!--Confirmar contraseña-->
          <InputType label="Confirmar contraseña*" id="confPwd" type="password" v-model="confPassword" />
          <!--Términos y condiciones-->
          <p class="mt-4 mb-4" style="font-size: small;">Al continuar, aceptas nuestros <a href="#">Términos de servicio</a> y <a href="#">Política de privacidad</a></p>
          <!--Iniciar sesión-->
          <div class="d-flex justify-content-center mt-4 mb-3">
            <p class="me-3 fs-6">¿Ya tienes una cuenta?</p>
            <p><a href="#">Inicie Sesión</a></p>
          </div>
          <!--Botón confirmar-->
          <div class="form-group text-center">
            <LinkButton @click="printValuesInConsole" text="Registrarse" />
          </div>
        </form>
      </div>
      <FooterComp />
    </div>
</template>

<script>
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
        firstName: "",
        secondName: "",
        firstSurname: "",
        secondSurname: "",
        idCard: "",
        number: "",
        selectedProvince: "",
        selectedCanton: "",
        selectedDistrit: "",
        otherSigns: "",
        birthday: "",
        email: "",
        password: "",
        confPassword: "",
        cantonesPorProvinciaCR: {
          "San José": {
            "San José": ["Carmen", "Merced", "Hospital", "Catedral", "Zapote", "San Francisco de Dos Ríos", "Uruca", "Mata Redonda", "Pavas", "Hatillo", "San Sebastián"],
            "Escazú": ["Escazú", "San Antonio", "San Rafael"],
            "Desamparados": ["Desamparados", "San Miguel", "San Juan de Dios", "San Rafael Arriba", "San Antonio", "Frailes", "Patarrá", "San Cristóbal", "Rosario", "Damas", "San Rafael Abajo", "Gravilias", "Los Guido"],
            "Puriscal": ["Santiago", "Mercedes Sur", "Barbacoas", "Grifo Alto", "San Rafael", "Candelarita", "Desamparaditos", "San Antonio", "Chires"],
            "Tarrazú": ["San Marcos", "San Lorenzo", "San Carlos"],
            "Aserrí": ["Aserrí", "Tarbaca", "Vuelta de Jorco", "San Gabriel", "Legua", "Monterrey", "Salitrillos"],
            "Mora": ["Colón", "Guayabo", "Tabarcia", "Piedras Negras", "Picagres", "Jaris", "Quitirrisí"],
            "Goicoechea": ["Guadalupe", "San Francisco", "Calle Blancos", "Mata de Plátano", "Ipis", "Rancho Redondo", "Purral"],
            "Santa Ana": ["Santa Ana", "Salitral", "Pozos", "Uruca", "Piedades", "Brasil"],
            "Alajuelita": ["Alajuelita", "San Josecito", "San Antonio", "Concepción", "San Felipe"],
            "Vásquez de Coronado": ["San Isidro", "San Rafael", "Dulce Nombre de Jesús", "Patalillo", "Cascajal"],
            "Acosta": ["San Ignacio", "Guaitil", "Palmichal", "Cangrejal", "Sabanillas"],
            "Tibás": ["San Juan", "Cinco Esquinas", "Anselmo Llorente", "León XIII", "Colima"],
            "Moravia": ["San Vicente", "San Jerónimo", "La Trinidad"],
            "Montes de Oca": ["San Pedro", "Sabanilla", "Mercedes", "San Rafael"],
            "Turrubares": ["San Pablo", "San Pedro", "San Juan de Mata", "San Luis", "Carara"],
            "Dota": ["Santa María", "Jardín", "Copey"],
            "Curridabat": ["Curridabat", "Granadilla", "Sánchez", "Tirrases"],
            "Pérez Zeledón": ["San Isidro de El General", "El General", "Rivas", "San Pedro", "Platanares", "Barú", "Río Nuevo", "Páramo"],
            "León Cortés": ["San Pablo"]
          },

          "Alajuela": {
            "Alajuela": [],
            "San Ramón": [],
            "Grecia": [],
            "San Mateo": [],
            "Atenas": [],
            "Naranjo": [],
            "Palmares": [],
            "Poás": [],
            "Orotina": [],
            "San Carlos": [],
            "Zarcero": [],
            "Sarchí": [],
            "Upala": [],
            "Los Chiles": [],
            "Guatuso": [],
            "Río Cuarto": []
          },

          "Cartago": {
            "Cartago": [],
            "Paraíso": [],
            "La Unión": [],
            "Jiménez": [],
            "Turrialba": [],
            "Alvarado": [],
            "Oreamuno": [],
            "El Guarco": []
          },

          "Heredia": {
            "Heredia": [],
            "Barva": [],
            "Santo Domingo": [],
            "Santa Bárbara": [],
            "San Rafael": [],
            "San Isidro": [],
            "Belén": [],
            "Flores": [],
            "San Pablo": [],
            "Sarapiquí": []
          },

          "Guanacaste": {
            "Liberia": [],
            "Nicoya": [],
            "Santa Cruz": [],
            "Bagaces": [],
            "Carrillo": [],
            "Cañas": [],
            "Abangares": [],
            "Tilarán": [],
            "Nandayure": [],
            "La Cruz": [],
            "Hojancha": []
          },

          "Puntarenas": {
            "Puntarenas": [],
            "Esparza": [],
            "Buenos Aires": [],
            "Montes de Oro": [],
            "Osa": [],
            "Quepos": [],
            "Golfito": [],
            "Coto Brus": [],
            "Parrita": [],
            "Corredores": [],
            "Garabito": [],
            "Monteverde": []
          },

          "Limón": {
            "Limón": [],
            "Pococí": [],
            "Siquirres": [],
            "Talamanca": [],
            "Matina": [],
            "Guácimo": []
          }
        },
      }
    },

    methods: {
      selectProvince() {
        return this.selectedProvince ? this.cantonesPorProvinciaCR[this.selectedProvince] : {};
      },

      selectCanton() {
        return this.selectedCanton ? this.cantonesPorProvinciaCR[this.selectedProvince][this.selectedCanton] : [];
      },

      printValuesInConsole() {
        console.log({
          firstName: this.firstName,
          secondName: this.secondName,
          firstSurname: this.firstSurname,
          secondSurname: this.secondSurname,
          idCard: this.idCard,
          number: this.number,
          selectedProvince: this.selectedProvince,
          selectedCanton: this.selectedCanton,
          selectedDistrit: this.selectedDistrit,
          otherSigns: this.otherSigns,
          birthday: this.birthday,
          email: this.email,
          password: this.password,
          confPassword: this.confPassword
        });
      }
    },
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
</style>
