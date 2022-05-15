import { model } from 'sdk'
import { AuthenticationMethod } from './AuthenticationMethod'

@model('PasswordAuthentication')
export class PasswordAuthentication extends AuthenticationMethod { }