<template>
  <v-data-table
    :headers="headers"
    :items="characters"
    :items-per-page="5"
    class="elevation-1"
    :loading="loading"
  >
    <template v-slot:top>
      <v-toolbar flat>
        <v-toolbar-title>Список персонажей</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-dialog v-model="dialog" max-width="500px">
          <template v-slot:activator="{ on, attrs }">
            <v-btn color="primary" dark class="mb-2" v-bind="attrs" v-on="on">
              Создать
            </v-btn>
          </template>
          <character-create-form
            @complite="closeDialog"
          ></character-create-form>
        </v-dialog>
        <v-dialog v-model="dialogDelete" max-width="500px">
          <v-card>
            <v-card-title class="headline">
              Are you sure you want to delete this item?
            </v-card-title>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" text @click="closeDelete">
                Cancel
              </v-btn>
              <v-btn color="blue darken-1" text @click="deleteItemConfirm">
                OK
              </v-btn>
              <v-spacer></v-spacer>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-toolbar>
    </template>
    <template v-slot:item.name="{ item }">
      <v-btn text color="primary" @click="openCharacter(item)">
        {{ item.name }}
      </v-btn>
    </template>
    <template v-slot:item.actions="{ item }">
      <v-icon small class="mr-2" @click="openCharacter(item)">
        mdi-open-in-new
      </v-icon>
      <v-icon small @click="deleteItem(item)">
        mdi-delete
      </v-icon>
    </template>
  </v-data-table>
</template>

<script>
import CharacterCreateForm from "@/character/CharacterCreateForm";
import { createNamespacedHelpers } from "vuex";

const { mapActions, mapGetters } = createNamespacedHelpers("character");

export default {
  components: {
    CharacterCreateForm
  },
  data() {
    return {
      dialog: false,
      editedIndex: null,
      dialogDelete: false,
      headers: [
        {
          text: "Персонаж",
          align: "start",
          sortable: false,
          value: "name"
        },
        { text: "Баланс", value: "balance" },
        { text: "Actions", value: "actions", sortable: false, align: "end" }
      ]
    };
  },
  computed: {
    ...mapGetters(["characters", "loading"])
  },
  methods: {
    ...mapActions(["loadCharacters", "deleteCharacter"]),
    loadData() {
      return this.loadCharacters();
    },
    openCharacter(character) {
      this.$router.push({
        name: "character",
        params: { id: character.id.toString() }
      });
    },
    closeDialog() {
      Promise.resolve(this.loadData()).finally(() => {
        this.dialog = false;
      });
    },
    deleteItem(item) {
      this.editedIndex = item.id;
      this.dialogDelete = true;
    },
    closeDelete() {
      this.dialogDelete = false;
      this.$nextTick(() => {
        this.editedIndex = null;
      });
    },
    deleteItemConfirm() {
      if (this.editedIndex) {
        const deletedCharacterId = this.editedIndex.toString();

        this.deleteCharacter(this.editedIndex).then(() => {
          this.closeDelete();

          if (
            this.$route.name === "character" &&
            this.$route.params.id === deletedCharacterId
          ) {
            this.$router.push({ name: "Characters" });
          }
        });
      }
    }
  },
  mounted: function() {
    this.loadData();
  }
};
</script>
