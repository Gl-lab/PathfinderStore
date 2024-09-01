<template>
  <v-row justify="space-around">
    <v-card width="800">
      <v-img
          height="200px"
          src="https://cdn.pixabay.com/photo/2020/07/12/07/47/bee-5396362_1280.jpg"
      ></v-img>

      <v-card-text>
        <div class="font-weight-bold ml-8 mb-2">
          Приветсвую
        </div>

        <v-timeline align-top dense>
          <v-timeline-item
              v-for="message in messages"
              :key="message.stepNumber"
              :color="message.color"
              small
          >
            <div>
              <div>{{ message.stepNumber }}.{{ message.message }}</div>
            </div>
          </v-timeline-item>
        </v-timeline>
      </v-card-text>
    </v-card>
  </v-row>
</template>

<script>
export default {
  data: () => ({
    messages: [
      {
        stepNumber: 1,
        message: `Создайте персонажей`,
        color: "green"
      },
      {
        stepNumber: 2,
        message: "Выберете текущего",
        color: "deep-purple lighten-1"
      },
      {
        stepNumber: 3,
        message: "...",
        color: "deep-purple lighten-1"
      },
      {
        stepNumber: 4,
        message: "Profit",
        color: "deep-purple lighten-1"
      }
    ]
  })
};
</script>
