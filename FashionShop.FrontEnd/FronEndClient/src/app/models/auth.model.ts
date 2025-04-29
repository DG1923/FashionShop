export interface LoginRequest{
    email: string;
    password: string;
}
export interface RegisterRequest{
    userName: string;
    email: string;
    password:string;
}
export interface AuthResponse {
    success: boolean;
    token: string;
    refreshToken: string;
    expiration: Date;
    message: string;
    userId: string;
    roles: string[];
}
export interface UserProfile{
    id: string;
    userName: string;
    email:string;
}