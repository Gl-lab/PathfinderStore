<template>
  <div id="app">
  <v-app>
      <v-navigation-drawer
      app
      clipped
      >
        <v-list>
          <v-list-item :to="{path: '/'}">
            <v-list-item-action>
              <v-icon>mdi-widgets</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>
                Домашняя
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-list-item :to="{path: '/vuetifyproduct'}">
          <v-list-item-action>
              <v-icon>mdi-home</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>
                Товары
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-list-item :to="{path: '/about'}">
          <v-list-item-action>
              <v-icon>mdi-heart</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>
                About
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list>
        <template v-slot:append>
          <div class="pa-2">
            <v-btn click="logout" block>Выйти</v-btn>
          </div>
        </template>
      </v-navigation-drawer>
    <v-app-bar 
      app
      clipped-left
    >
      <v-toolbar-title>App Bar</v-toolbar-title>
    </v-app-bar>
    <v-content>
      <v-container fluid>
        <v-fade-transition mode="out-in">
          <router-view></router-view>
          
        </v-fade-transition>
      </v-container>
    </v-content>   

  </v-app>
  </div>
</template>

<script src="./App.ts"></script>