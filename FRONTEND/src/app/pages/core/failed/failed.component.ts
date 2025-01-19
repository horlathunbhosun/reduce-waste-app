import { Component } from '@angular/core';
import {MatCard, MatCardContent, MatCardTitle} from "@angular/material/card";

@Component({
  selector: 'app-failed',
  standalone: true,
  imports: [
    MatCard,
    MatCardContent,
    MatCardTitle
  ],
  templateUrl: './failed.component.html',
  styleUrl: './failed.component.scss'
})
export class FailedComponent {

}
