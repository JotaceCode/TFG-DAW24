import { ApplicationRef, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject, filter, take } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  urlBase = environment.gatewayUrl;
  private hubConnection: HubConnection | any;
  public notifications$: Subject<string> = new Subject<string>();

  constructor(private appRef: ApplicationRef) {
    this.createHubConnection();
  }

  private createHubConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.urlBase + 'notificationHub/notificationHub') // URL del hub SignalR
      .build();

    this.hubConnection.on('ReceiveNotification', (data: any) => {
      this.notifications$.next(data);
    });

    this.appRef.isStable
      .pipe(
        // Filtrar los eventos hasta que la aplicación esté completamente estable
        filter((isStable) => isStable),
        // Tomar el primer evento de estabilidad (es decir, cuando la aplicación esté completamente estable)
        take(1)
      )
      .subscribe(() => {
        // Conectar a SignalR solo cuando la aplicación esté completamente estable
        if (typeof localStorage !== 'undefined') {
          if (localStorage.getItem('token')) {
            if (localStorage.getItem('rol') === 'true') {
              this.hubConnection
                .start()
                .then(() =>
                  console.log('Conexión establecida con el servidor de SignalR')
                )
                .catch(() =>
                  console.error('Error al conectar con el servidor de SignalR:')
                );
            }
          }
        }
      });
  }
}
