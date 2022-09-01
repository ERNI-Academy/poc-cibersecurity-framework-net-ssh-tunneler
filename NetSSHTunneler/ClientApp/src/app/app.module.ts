import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { ConnectionFormComponent } from './connection-form/connection-form.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Material
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon'
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { HomeServiceApi } from './api/home.api';
import { environment } from 'src/environments/environment';
import { DialogSimpleMessage } from './dialogs/dialog-simple-message/dialog-simple-message.component';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { DashboardComponent } from './dashboard/dashboard.component';
import { SshTerminalComponent } from './ssh-terminal/ssh-terminal.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { CommandServiceApi } from './api/command.api';
import { DiscoveryComponent } from './discovery/discovery.component';
import { NetworkMapComponent } from './network-map/network-map.component';
import { NetworkServiceApi } from './api/network.api';
import { MatTreeModule } from '@angular/material/tree';
import { CommonService } from './api/common.service';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { SettingsScreenComponent } from './settings-screen/settings-screen.component';
import { MatRadioModule } from '@angular/material/radio';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { LoadDiscoveryConfigurationDialog } from './dialogs/discovery-configuration-load/discovery-configuration-load.component';
import { RunDiscoveryComponent } from './run-discovery/run-discovery.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { DashboardDataService } from './dashboard/dashboard.service';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        CounterComponent,
        ConnectionFormComponent,
        DialogSimpleMessage,
        LoadDiscoveryConfigurationDialog,
        DashboardComponent,
        SshTerminalComponent,
        DiscoveryComponent,
        NetworkMapComponent,
        SettingsScreenComponent,
        RunDiscoveryComponent
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', component: DashboardComponent, pathMatch: 'full' },
            { path: 'discovery', component: DiscoveryComponent },
            { path: 'network', component: NetworkMapComponent },
            { path: 'settings', component: SettingsScreenComponent },
            { path: 'run', component: RunDiscoveryComponent },
            { path: 'new', component: ConnectionFormComponent },
        ]),
        BrowserAnimationsModule,
        MatCardModule,
        MatExpansionModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatButtonModule,
        MatDialogModule,
        DragDropModule,
        MatProgressBarModule,
        MatTreeModule,
        MatSlideToggleModule,
        MatRadioModule,
        MatSnackBarModule,
        MatTooltipModule,
        MatMenuModule
    ],
    providers: [
        HomeServiceApi,
        CommandServiceApi,
        NetworkServiceApi,
        CommonService,
        DashboardDataService,
        { provide: "API_BASE_URL", useValue: environment.apiRoot }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
