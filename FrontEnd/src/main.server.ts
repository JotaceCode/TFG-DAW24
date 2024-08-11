import { bootstrapApplication } from '@angular/platform-browser';
import { RootComponent } from './app/root.component';
import { config } from './app/app.config.server';

const bootstrap = () => bootstrapApplication(RootComponent, config);

export default bootstrap;
