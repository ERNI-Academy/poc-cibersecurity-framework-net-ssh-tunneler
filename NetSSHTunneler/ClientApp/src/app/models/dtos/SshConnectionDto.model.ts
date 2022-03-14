export class SshConnectionDto {
    public TargetIp: string;
    public TargetPort: string;
    public UserName: string;
    public Password: string;
    public Certificate: string;
    public AdditionalSshParameters: string;
  }