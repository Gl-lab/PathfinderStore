<template>
    <div v-if="!registerComplete">
        <v-card class="elevation-12">
            <v-toolbar dark color="primary">
                <v-toolbar-title>Register</v-toolbar-title>
            </v-toolbar>
            <v-card-text>
                <div v-for="error in errors" :key="error.name">
                    <v-alert :value="true" type="error">
                        {{error.name}}
                    </v-alert>
                </div>
                <v-form ref="form" @keyup.native.enter="onSubmit">
                    <v-text-field name="userName" label='UserName' type="text"
                                  v-model="registerInput.userName"
                                  :rules="[requiredError]"></v-text-field>
                    <v-text-field name="email" label='EmailAddress' type="text"
                                  v-model="registerInput.email"
                                  :rules="[requiredError,emailError]"></v-text-field>
                    <v-text-field name="password" label='Password' type="password"
                                  v-model="registerInput.password"
                                  :rules="[requiredError]"></v-text-field>
                    <v-text-field name="passwordRepeat" label='PasswordRepeat'
                                  v-model="registerInput.passwordRepeat" type="password"
                                  :rules="[requiredError]"
                                  :error-messages="passwordMatchError(registerInput.password,registerInput.passwordRepeat)"></v-text-field>
                </v-form>
            </v-card-text>
            <v-card-actions class="pa-5">
                <v-spacer></v-spacer>
                <v-btn color="primary" text to="/account/login">Login</v-btn>
                <v-btn color="primary" @click="onSubmit">Register</v-btn>
            </v-card-actions>
        </v-card>
    </div>
    <div v-else>
        <v-card class="elevation-12">
            <v-toolbar dark color="primary">
                <v-toolbar-title>Register</v-toolbar-title>
            </v-toolbar>
            <v-card-text>
                <v-alert :value="true" type="success">
                    {{resultMessage}}
                </v-alert>
            </v-card-text>
            <v-card-actions class="pa-5">
                <v-spacer></v-spacer>
                <v-btn color="primary" text to="/account/login">Login</v-btn>
            </v-card-actions>
        </v-card>
    </div>
</template>

<script src="./register.ts"></script>