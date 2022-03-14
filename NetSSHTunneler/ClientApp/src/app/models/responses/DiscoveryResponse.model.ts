import { CommandContainer } from "../models/CommandContainer.model";
import { CommandResponse } from "./CommandResponse.model";

export class DiscoveryScriptFolder {
  public name: string;
  public scripts: DiscoveryScriptFile[];
}

export class DiscoveryScriptFile {
  public name: string;
  public path: string;
  public content: CommandContainer;
}

export class DiscoveryResults {
  public commandResponses: CommandResponse[];
}