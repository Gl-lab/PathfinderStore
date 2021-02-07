<template>
  <v-data-table
    :headers="headers"
    :items="list"
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
              New Item
            </v-btn>
          </template>
          <character-create-form
            @complite="closeDialog"
          ></character-create-form>
        </v-dialog>
        <v-dialog v-model="dialogDelete" max-width="500px">
          <v-card>
            <v-card-title class="headline"
              >Are you sure you want to delete this item?</v-card-title
            >
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" text @click="closeDelete"
                >Cancel</v-btn
              >
              <v-btn color="blue darken-1" text @click="deleteItemConfirm"
                >OK</v-btn
              >
              <v-spacer></v-spacer>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-toolbar>
    </template>
    <template v-slot:item.actions="{ item }">
      <v-icon small @click="deleteItem(item)">
        mdi-delete
      </v-icon>
    </template>
  </v-data-table>
</template>

<script>
import CharacterCreateForm from "@/character/CharacterCreateForm";

export default {
  components: {
    CharacterCreateForm
  },
  data() {
    return {
      loading: false,
      dialog: false,
      list: [],
      editedIndex: null,
      dialogDelete: false,
      editedItem: null,
      headers: [
        {
          text: "Персонаж",
          align: "start",
          sortable: false,
          value: "name"
        },
        { text: "Баланс", value: "balance" },
        { text: "Actions", value: "actions", sortable: false }
      ]
    };
  },
  methods: {
    loadData() {
      this.loading = true;
      this.axios
        .get("api/GameAccount", this.pageInfo)
        .then(response => {
          this.list = response.data.characters;
        })
        .catch(() => (this.list = []))
        .then(() => (this.loading = false));
    },
    closeDialog() {
      this.dialog = false;
      this.loadData();
    },
    deleteItem(item) {
      this.editedIndex = this.list.indexOf(item);
      this.editedItem = item;
      this.dialogDelete = true;
    },
    closeDelete() {
      this.dialogDelete = false;
      this.$nextTick(() => {
        this.editedItem = null;
        this.editedIndex = -1;
      });
    },
    deleteItemConfirm() {
      this.axios
        .delete("/api/Character", {
          params: { deletedCharacter: this.editedItem.id }
        })
        .then(() => {
          this.list.splice(this.editedIndex, 1);
          this.closeDelete();
        });
    }
  },
  mounted: function() {
    this.loadData();
  }
};
</script>
