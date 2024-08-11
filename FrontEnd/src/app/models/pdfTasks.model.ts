import { Room } from './room.model';
import { User } from './user.model';

export interface pdfTask {
  Id?: string;
  id?: string;
  User: User;
  user?: User;
  Room: Room;
  room?: Room;
  Priority?: string;
  priority?: string;
  Observations?: string;
  observations?: string;
  Status?: string;
  status?: string;
  createdDate?: Date;
}
