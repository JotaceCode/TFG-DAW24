import { Routes } from '@angular/router';
import { UserTableComponent } from './components/admin/user/user.component';
import { LoginComponent } from './components/security/login/login.component';
import { DashboardComponent } from './components/shared/dashboard/dashboard.component';
import { RoomTableComponent } from './components/admin/room/room.component';
import {
  SecurityNoRolesRouter,
  SecurityRolesRouter,
  SecurityRouter,
} from './components/security/security.router';
import { TaskTableComponent } from './components/admin/task/task.component';
import { ProfileComponent } from './components/shared/profile/profile.component';
import { CalendarComponent } from './components/shared/calendar/calendar.component';
import { TasksUsersComponent } from './components/user/tasksUser.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  {
    path: 'inicio',
    component: DashboardComponent,
    canActivate: [SecurityRouter],
  },
  {
    path: 'mistareas',
    component: TasksUsersComponent,
    canActivate: [SecurityRouter, SecurityNoRolesRouter],
  },
  {
    path: 'tareas',
    component: TaskTableComponent,
    canActivate: [SecurityRouter, SecurityRolesRouter],
  },
  {
    path: 'empleados',
    component: UserTableComponent,
    canActivate: [SecurityRouter, SecurityRolesRouter],
  },
  {
    path: 'habitaciones',
    component: RoomTableComponent,
    canActivate: [SecurityRouter, SecurityRolesRouter],
  },
  {
    path: 'calendario',
    component: CalendarComponent,
    canActivate: [SecurityRouter],
  },
  {
    path: 'perfil',
    component: ProfileComponent,
    canActivate: [SecurityRouter],
  },
  { path: '**', redirectTo: '' },
];
