export interface Task {
  Id?: string | any;
  id?: string | any; // For watch json is lowercase
  User: TaskUser;
  Room: TaskRoom;
  room?: TaskRoom | any; // For watch json is lowercase
  Priority?: string;
  priority?: string; // For watch json is lowercase
  Observations?: string;
  observations?: string; // For watch json is lowercase
  Status?: string | any;
  status?: string | any; // For watch json is lowercase
  createdDate?: Date;
}

export interface TaskUser {
  Id: string;
}

export interface TaskRoom {
  Id?: string;
  roomNumber: string;
  Floor?: string;
}
