export interface InsertOrUpdateUser {
  Password?: string; // only when insert new user
  Name: string;
  Surname: string;
  Email: string;
  DNI: string;
  PhoneNumber: string;
  BirthDate?: Date | any;
  CP: string;
  City: string;
  RoleAdmin: boolean;
  urlImage: string;
  Disabled?:boolean;
}
