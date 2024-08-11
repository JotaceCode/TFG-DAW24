import { Component, Injectable, NgZone, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-clock',
  standalone: true,
  template: `{{ formattedDate }}`,
})
@Injectable({
  providedIn: 'root',
})
export class clockComponent implements OnDestroy {
  public formattedDate: string = '';
  private animationFrameId: number | null = null;

  constructor(private ngZone: NgZone) {
    this.updateFormattedDate();
    this.scheduleNextUpdate();
  }

  // Format for datetime
  private updateFormattedDate() {
    const now = new Date();
    const formattedDate = this.formatDate(now);
    const formattedTime = this.formatTime(now);
    this.formattedDate = `${formattedDate} - ${formattedTime}`;
  }

  // format for date
  private formatDate(date: Date): string {
    const day = date.getDate();
    const month = this.getMonthName(date.getMonth());
    const year = date.getFullYear();
    return `${day} de ${month} de ${year}`;
  }

  // format for time
  private formatTime(date: Date): string {
    let hours = date.getHours();
    const minutes = this.padZero(date.getMinutes());
    const ampm = hours >= 12 ? 'p.m.' : 'a.m.';
    hours = hours % 12;
    hours = hours ? hours : 12; // La hora '0' debería ser '12'
    return `${hours}:${minutes} ${ampm}`;
  }

  // method for get the month name
  private getMonthName(month: number): string {
    const months = [
      'enero',
      'febrero',
      'marzo',
      'abril',
      'mayo',
      'junio',
      'julio',
      'agosto',
      'septiembre',
      'octubre',
      'noviembre',
      'diciembre',
    ];
    return months[month];
  }

  // Método para añadir ceros a la izquierda si es necesario
  private padZero(num: number): string {
    return num < 10 ? '0' + num : num.toString();
  }

  // Update the time using requestAnimationFrame
  private scheduleNextUpdate() {
    this.ngZone.runOutsideAngular(() => {
      this.animationFrameId = requestAnimationFrame(() => {
        this.ngZone.run(() => {
          this.updateFormattedDate();
          this.scheduleNextUpdate();
        });
      });
    });
  }

  // Reset when destroy the component
  ngOnDestroy(): void {
    if (this.animationFrameId !== null) {
      cancelAnimationFrame(this.animationFrameId);
    }
  }
}
