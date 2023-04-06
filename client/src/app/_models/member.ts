import { photo } from "./photo";

export interface Member {
  id: number;
  userName: string;
  photoUrl: string;
  age: number;
  knownAs: string;
  created: string;
  lastActive: string;
  gender: string;
  lookingFor: string;
  interest: any;
  city: string;
  country: string;
  photos: photo[];
}


