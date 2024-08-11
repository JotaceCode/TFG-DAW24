export interface User{
  id: string | any;
  name:string;
  surname:string;
  email: string;
  dni:string;
  phoneNumber: string;
  birthDate: Date;
  cp:string;
  city:string;
  roleAdmin:boolean;
  token:string;
  urlImage: string;
  disabled?:boolean;
}
