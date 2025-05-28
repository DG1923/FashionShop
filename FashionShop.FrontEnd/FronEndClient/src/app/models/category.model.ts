export interface CategoryDisplayDto {
  id: string;
  name: string;
  imageUrl: string;
  link: string;
  gender: 'male' | 'female';
}
export interface Category {
    id: string;
    name: string;
    imageUrl: string;
    subCategory?: Category[];
}